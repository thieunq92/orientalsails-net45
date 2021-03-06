using System;
using System.Globalization;
using System.IO;
using System.Web;
using CMS.Web.Util;
using log4net;

//using log4net;

namespace CMS.Web.HttpHandlers
{
    public class ImageHandler : IHttpHandler
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof (ImageHandler));

        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest req = context.Request;
            string path = req.PhysicalPath;

            #region -- Check if image is refer from another host --
            if (req.UrlReferrer != null && req.UrlReferrer.Host.Length > 0)
            {
//                _logger.Error(req.UrlReferrer.Host);
                if (CultureInfo.InvariantCulture.CompareInfo.Compare(req.Url.Host,req.UrlReferrer.Host, CompareOptions.IgnoreCase) != 0)
                {
                    //TODO: Watermark picture
                    //path = context.Server.MapPath(GlobalConst.NO_IMAGE);
                    context.Response.Status = "Image not found";
                    context.Response.StatusCode = 404;
                    return;
                }
            }
            #endregion

            #region -- Get file info --
            // Lấy file info từ đường dẫn
            FileInfo file = new FileInfo(path);
            if (!file.Exists)
            {
                _logger.Error(string.Format("Image {0} not found, referer: {1}",req.Url,req.UrlReferrer));
                path = context.Server.MapPath(GlobalConst.NO_IMAGE);
            }
            string contentType;
            string extension = Path.GetExtension(path).ToLower();
            switch (extension)
            {
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".jpg":
                    contentType = "image/jpeg";
                    break;
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                default:
                    throw new NotSupportedException("Unrecognized image type.");
            }
            #endregion

            if (!File.Exists(path))
            {
                context.Response.Status = "Image not found";
                context.Response.StatusCode = 404;
            }
            else
            {
                // Luôn hiển thị cache trong 1 giờ = 60*60 giây đầu
                // Kiểm tra nội dung cache sau 48 giờ = 60*60*24 giây sau
                // Client behavior
                context.Response.Cache.AppendCacheExtension("post-check=3600,pre-check=172800");

                // Nếu là file noimage, đổi lại FileInfo
                if (!file.Exists)
                {
                    file = new FileInfo(path);
                }

                context.Response.Cache.SetETag(file.LastWriteTimeUtc.ToString("r", DateTimeFormatInfo.InvariantInfo));
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetMaxAge(new TimeSpan(7,0,0,0));
                context.Response.Cache.SetSlidingExpiration(true); // Để gửi etag

                context.Response.Cache.SetLastModified(file.LastWriteTimeUtc);

                #region -- Check --
                if (HasCache(context, file))
                {
                    context.Response.StatusCode = 304;
                    context.Response.ContentType = contentType;
                    context.Response.End();
                    return;
                }
                // Nếu chưa cache
                context.Response.StatusCode = 200;
                context.Response.ContentType = contentType;
                #endregion

                context.Response.WriteFile(path);
                context.Response.End();
            }
        }

        private static bool HasCache(HttpContext context, FileSystemInfo file)
        {
            // Chỉ khi không có Etag thì mới check Modified Date
            if (string.IsNullOrEmpty(context.Request.Headers["If-None-Match"]))
            {
                string incomingTime = context.Request.Headers["If-Modified-Since"];
                DateTime inTime = Convert.ToDateTime(incomingTime);
                return inTime.AddDays(1).AddSeconds(1) >= file.LastWriteTimeUtc;
            }
            string eTagString = file.LastWriteTimeUtc.ToString("r", DateTimeFormatInfo.InvariantInfo);
            return (string.Compare(eTagString, context.Request.Headers["If-None-Match"]) == 0);
        }

        public bool IsReusable
        {
            get { return true; }
        }

        #endregion
    }
}