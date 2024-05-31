using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace NhomBoy_Hotel.Controllers
{
    public class NhanVienController : Controller
    {
        DatPhongKhachSanEntities db = new DatPhongKhachSanEntities();
        // GET: NhanVien
        public ActionResult DuyetPhong(string loai)
        {
            var ds = db.DatPhongs.ToList();
            if (loai == "chuaDuyet")
                ds = db.DatPhongs.Where(t => t.TrangThai == "Chưa duyệt").ToList();
            if (loai == "chuaNhan")
                ds = db.DatPhongs.Where(t => t.TrangThai == "Chưa nhận phòng").ToList();
            if (loai == "chuaTra")
                ds = db.DatPhongs.Where(t => t.TrangThai == "Đã nhận phòng").ToList();
            if (loai == "daTra")
                ds = db.DatPhongs.Where(t => t.TrangThai == "Đã trả phòng").ToList();
            return View(ds);
        }
        public ActionResult Duyet(int? MaDatPhong)
        {

            DatPhong dp = db.DatPhongs.FirstOrDefault(t => t.MaDatPhong == MaDatPhong);
            Phong p = db.Phongs.FirstOrDefault(t => t.MaPhong == dp.MaPhong);
            KhachHang kh = db.KhachHangs.FirstOrDefault(t => t.MaKhachHang == dp.MaKhachHang);
            string Email = kh.Email;
            if (dp.TrangThai == "Đã nhận phòng")
                dp.TrangThai = "Đã trả phòng";
            if (dp.TrangThai == "Chưa nhận phòng")
                dp.TrangThai = "Đã nhận phòng";
            if (dp.TrangThai == "Chưa duyệt")
            {
                dp.TrangThai = "Chưa nhận phòng";
                SendingEmail(Email, kh.HoTen, p.SoPhong, (DateTime)dp.NgayDat);

            }

            db.SaveChanges();
            var ds = db.DatPhongs.ToList();
            return RedirectToAction("DuyetPhong",ds);
        }
        public ActionResult Xoa(int? MaDatPhong)
        {
            DatPhong dp = db.DatPhongs.FirstOrDefault(t => t.MaDatPhong == MaDatPhong);
            var p = db.PhieuDichVus.Where(t => t.MaDatPhong == MaDatPhong).ToList();
            db.PhieuDichVus.RemoveRange(p);
            db.DatPhongs.Remove(dp);
            db.SaveChanges();
            var ds = db.DatPhongs.ToList();
            return RedirectToAction("DuyetPhong", ds);
        }
        public ActionResult QuanLyPhong()
        {
            var ds = db.Phongs.ToList();
            return View(ds);
        }
        public ActionResult ThemPhong()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ThemPhong(Phong phong, HttpPostedFileBase HinhAnh)
        {
            if (HinhAnh != null && HinhAnh.ContentLength > 0)
            {
                // Đặt đường dẫn thư mục lưu file
                var path = Path.Combine(Server.MapPath("~/img"), Path.GetFileName(HinhAnh.FileName));

                // Lưu file vào thư mục
                HinhAnh.SaveAs(path);

                phong.HinhAnh =  Path.GetFileName(HinhAnh.FileName);
            }
            db.Phongs.Add(phong);
               db.SaveChanges();
               return RedirectToAction("QuanLyPhong", "NhanVien");
           }

        public ActionResult XoaPhong(int? MaPhong)
        {
            Phong p = db.Phongs.Find(MaPhong);
            db.Phongs.Remove(p);
            db.SaveChanges();
            return RedirectToAction("QuanLyPhong", "NhanVien");
        }
        public void SendingEmail(String to, String ten, String phong, DateTime ngay)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("a0934805315@gmail.com");
            mailMessage.To.Add(to);
            mailMessage.Subject = phong;
            mailMessage.IsBodyHtml = true;

            String htmlBody = CreateHtmlBody(ten, phong, ngay);

            mailMessage.Body = htmlBody;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential("a0934805315@gmail.com", "qsuhfxfzlzkkkrse");
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("Email Sent Successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private String CreateHtmlBody(String ten, String phong, DateTime ngay)
        {
            String ngayString = ngay.ToString("yyyy-MM-dd"); // Định dạng DateTime thành chuỗi
            String htmlBody = System.IO.File.ReadAllText(Server.MapPath("~/email_template.html"));
            htmlBody = htmlBody.Replace("${ten}", ten);
            htmlBody = htmlBody.Replace("${phong}", phong);
            htmlBody = htmlBody.Replace("${ngay}", ngayString);

            return htmlBody;
        }

        public ActionResult QuanLyDichVu()
        {
            var ds = db.DichVus.ToList();
            return View(ds);
        }
        public ActionResult ThemDV()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemDV(DichVu dv, HttpPostedFileBase HinhAnh)
        {
            if (HinhAnh != null && HinhAnh.ContentLength > 0)
            {
                // Đặt đường dẫn thư mục lưu file
                var path = Path.Combine(Server.MapPath("~/img"), Path.GetFileName(HinhAnh.FileName));

                // Lưu file vào thư mục
                HinhAnh.SaveAs(path);

                dv.HinhAnh = Path.GetFileName(HinhAnh.FileName);
            }
            db.DichVus.Add(dv);
            db.SaveChanges();
            return RedirectToAction("QuanLyDichVu", "NhanVien");
        }
        public ActionResult XoaDV(int? MaDichVu)
        {
            DichVu p = db.DichVus.Find(MaDichVu);
            db.DichVus.Remove(p);
            db.SaveChanges();
            return RedirectToAction("QuanLyDichVu", "NhanVien");
        }
    }
}