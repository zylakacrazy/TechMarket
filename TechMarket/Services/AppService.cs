using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechMarket.Hubs;
using TechMarket.Models;

namespace TechMarket.Services
{
    public class AppService
    {
        SGDFleaMarketEntities _Context;

        public AppService()
        {
            _Context = new SGDFleaMarketEntities();
        }

        public bool Login(LoginData loginData, out int userId)
        {
            userId = 0;
            string giaima = CreateMD5.EncodePassword(loginData.Password);
            var currentUser = _Context.tbl_Account.FirstOrDefault(x => x.username == loginData.Username && x.password == giaima);
            if (currentUser != null)
            {
                userId = currentUser.Id;
                return true;
            }
            return false;
        }

        public List<UserDTO> GetUsersToChat()
        {
            var userId = int.Parse(HttpContext.Current.User.Identity.Name);
            var mss = _Context.tbl_Messages.Where(x => x.formUser == userId||x.toUser == userId).ToList();
            List<tbl_Account> account = _Context.tbl_Account.ToList();
            foreach (var item in mss)
            {
                account = _Context.tbl_Account.Include("tbl_Channel").Where(x => x.Id == item.toUser || x.Id == item.formUser).ToList();
            }
            return account
                .Where(x => x.Id != userId)
                .Select(x => new UserDTO
                    {
                        UserId = x.Id,
                        UserName = x.username,
                        FullName = x.fullname,
                        IsOnline = x.tbl_Channel.Count > 0,
                    }).ToList();
        }

        internal int AddUserConnection(Guid ConnectionId)
        {
            var userId = int.Parse(HttpContext.Current.User.Identity.Name);
            _Context.tbl_Channel.Add(new tbl_Channel
            {
                ConnectionId = ConnectionId,
                userId = userId,
            });
            _Context.SaveChanges();
            return userId;
        }

        internal int RemoveUserConnection(Guid ConnectionId)
        {
            int userId = 0;
            var current = _Context.tbl_Channel.FirstOrDefault(x => x.ConnectionId == ConnectionId);
            if (current != null)
            {
                userId = current.userId ?? 0;
                _Context.tbl_Channel.Remove(current);
                _Context.SaveChanges();
            }
            return userId;
        }

        internal IList<string> GetUSerConnections(int uSerId)
        {
            return _Context.tbl_Channel.Where(x => x.userId == uSerId).Select(x => x.ConnectionId.ToString()).ToList();
        }

        internal void RemoveAllUserConnections(int userId)
        {
            var current = _Context.tbl_Channel.Where(x => x.userId == userId);
            _Context.tbl_Channel.RemoveRange(current);
            _Context.SaveChanges();
        }

        internal ChatBoxModel GetChatbox(int toUserId)
        {
            var userId = int.Parse(HttpContext.Current.User.Identity.Name);
            var toUser1 = _Context.tbl_Account.FirstOrDefault(x => x.Id == toUserId);
            var messages = _Context.tbl_Messages.Where(x => (x.formUser == userId && x.toUser == toUserId) || (x.formUser == toUserId && x.toUser == userId))
                .OrderByDescending(x => x.messages_time)
                .Skip(0)
                .Take(50)
                .Select(x => new MessageDTO
                {
                    ID = x.Id,
                    Message = x.messages,
                    Class = x.formUser == userId ? "from" : "to",
                })
                .OrderBy(x => x.ID)
                .ToList();

            return new ChatBoxModel
            {
                ToUser = ToUserDTO(toUser1),
                Messages = messages,
            };
        }

        internal bool SendMessage(int toUserId, string message)
        {
            try
            {
                int USER_ID = int.Parse(HttpContext.Current.User.Identity.Name);
                _Context.tbl_Messages.Add(new tbl_Messages
                {
                    formUser = USER_ID,
                    toUser = toUserId,
                    messages = message,
                    messages_time = DateTime.Now
                });
                _Context.SaveChanges();
                ChatHub.RecieveMessage(USER_ID, toUserId, message);
                return true;
            }
            catch { return false; }
        }

        public UserDTO ToUserDTO(tbl_Account user)
        {
            return new UserDTO
            {
                FullName = user.fullname,
                UserId = user.Id,
                UserName = user.username,
            };
        }

        internal List<MessageDTO> LazyLoadMssages(int toUserId, int skip)
        {
            var userId = int.Parse(HttpContext.Current.User.Identity.Name);
            var messages = _Context.tbl_Messages.Where(x => (x.formUser == userId && x.toUser == toUserId) || (x.formUser == toUserId && x.toUser == userId))
                .OrderByDescending(x => x.messages_time)
                .Skip(skip)
                .Take(50)
                .Select(x => new MessageDTO
                {
                    ID = x.Id,
                    Message = x.messages,
                    Class = x.formUser == userId ? "from" : "to",
                })
                .OrderByDescending(x => x.ID)
                .ToList();
            return messages;
        }
    }
}