using Dake.Models;
using Dake.Utility;
using Dake.ViewModel;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service.Interface
{
    public interface INotice
    {
        PagedList<Notice> GetNotices(string currentUser, int? filtercategory,int pageId = 1, string filterTitle = "");

        object GetNotices(int page = 1);
        //object GetNoticesByCatAndType(NoticeSearch NoticeSearch);
        object GetLastNotices(int page = 1);
        //int NoticeFromAdmin(CreateNoticeViewModel Notice);
        long Notice(Notice Notice);
        //void EditNotice(CreateNoticeViewModel Notice);
        object GetNotices(string Token,int page = 1, int pagesize = 10 );
        //object GetAllEspacialNotices(string Token, int page, int noticeId, string scroll);
        object GetAllEspacialNotices(string Token, long noticeId, string scroll);

        int GetAdminAcceptedNoticeCount(string fromd, string tod, string username);

        //object GetNoticesByTitle(NoticeSearch2 NoticeSearch);
        //object NoticeToFactor(BuyNotice buyNotice);
        //object GetFactorOfUser(GetFactor getFactor);
        //object GetLinkOfNotices(GetLinkOfNotice getLinkOfNotice);
        //object GetFactorItems(AllFactorItem allFactorItem);
    }
}
