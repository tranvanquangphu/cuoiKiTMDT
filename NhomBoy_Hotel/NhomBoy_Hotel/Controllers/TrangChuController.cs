using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

namespace NhomBoy_Hotel.Controllers
{
    public class TrangChuController : Controller
    {
        DatPhongKhachSanEntities db = new DatPhongKhachSanEntities();
        // GET: TrangChu
        public ActionResult TrangChu(int? MaKhachHang,decimal? loai)
        {
            Session["PreviousUrl"] = Request.Url.ToString();
            if (MaKhachHang == null)
                MaKhachHang = Int32.Parse(Request.QueryString["MaKhachHang"]);
            var ds = db.Phongs.ToList();              
            if (loai == null ||loai==0)
                ds = db.Phongs.ToList();
            else
                ds = db.Phongs.Where(t => t.GiaMoiDem < loai&& t.GiaMoiDem>loai-400000).ToList();
            if (loai == -1)
                ds = db.Phongs.Where(t => t.GiaMoiDem > 1000000).ToList();
            ViewBag.MaKhachHang = MaKhachHang;
            return View(ds);
        }
        public ActionResult XemChiTiet(int? MaPhong, int? MaKhachHang)
        {
            Session["PreviousUrl"] = Request.Url.ToString();
            Phong Phong = db.Phongs.FirstOrDefault(t => t.MaPhong == MaPhong);
            var dsAnh = db.HinhAnhPhongs.Where(t => t.MaPhong == MaPhong).ToList();
            ViewBag.MaKhachHang = MaKhachHang;
            string starSymbols = new string('★', 3) + new string('☆', 5 - 3); // Full stars and empty stars
            ViewBag.Sao = starSymbols;
            ViewBag.dsAnh = dsAnh;
            return View(Phong);
        }
        public ActionResult DienThongTin(int? MaPhong, int? MaKhachHang)
        {
            Phong phong = db.Phongs.FirstOrDefault(t => t.MaPhong == MaPhong);
            ViewBag.MaPhong = MaPhong;
            if (MaKhachHang == null)
            {

                return RedirectToAction("DangNhap_DangKy", "DangNhap_DangKy");
            }
            ViewBag.SoPhong = phong.SoPhong;
            ViewBag.hinhAnh = phong.HinhAnh;
            ViewBag.SoTien = phong.GiaMoiDem;
            ViewBag.Code = MaKhachHang + "_" + phong.SoPhong;
            KhachHang kh = db.KhachHangs.FirstOrDefault(t => t.MaKhachHang == MaKhachHang);
            return View(kh);

        }
        [HttpPost]
        public ActionResult XacNhanThanhToan(int? MaPhong, int? MaKhachHang, DateTime NgayNhanPhong, DateTime NgayTraPhong, int? SoNguoi, string phuongThucThanhToan, string Code)
        {

            Phong phong = db.Phongs.Find(MaPhong);
            KhachHang kh = db.KhachHangs.FirstOrDefault(t => t.MaKhachHang == MaKhachHang);
            if (MaKhachHang == null)
                return RedirectToAction("DangNhap_DangKy", "DangNhap_DangKy");
            DatPhong dp = new DatPhong();
            dp.MaKhachHang = MaKhachHang;
            dp.MaPhong = MaPhong;
            dp.NgayDat = DateTime.Now;
            dp.NgayNhanPhong = NgayNhanPhong;
            dp.NgayTraPhong = NgayTraPhong;
            dp.SoLuongNguoi = SoNguoi;
            dp.TongTien = phong.GiaMoiDem * (NgayTraPhong - NgayNhanPhong).Days;
            if (phuongThucThanhToan == "trucTuyen")
                dp.TrangThaiThanhToan = "Thanh toán trực tuyến";
            else
                dp.TrangThaiThanhToan = "Thanh toán tại khách sạn";
            dp.TrangThai = "Chưa duyệt";
            dp.code = Code;
            db.DatPhongs.Add(dp);
            db.SaveChanges();
            int? MaDatPhong = db.DatPhongs.FirstOrDefault(t => t.MaPhong == MaPhong && t.MaKhachHang == MaKhachHang).MaDatPhong;
            return RedirectToAction("DangKyDichVu", new {MaDatPhong=MaDatPhong});
        }
        public ActionResult DichVu()
        {
            return View();
        }
        public ActionResult PhongDaDat(int? MaKhachHang)
        {
            ViewBag.MaKhachHang = MaKhachHang;
            var ds = db.DatPhongs.Where(t => t.MaKhachHang == MaKhachHang).ToList();
            return View(ds);
        }
        public JsonResult KiemTra(int? MaPhong, int MaKhachHang, DateTime NgayNhanPhong, DateTime NgayTraPhong, int? SoNguoi)
        {
            var ktr = db.DatPhongs.Where(dp => dp.MaPhong == MaPhong && ((NgayNhanPhong >= dp.NgayNhanPhong && NgayNhanPhong < dp.NgayTraPhong) || (NgayTraPhong > dp.NgayNhanPhong && NgayTraPhong <= dp.NgayTraPhong) || (NgayNhanPhong <= dp.NgayNhanPhong && NgayTraPhong >= dp.NgayTraPhong))).Any();
            if (ktr)
            {
                return Json(new { success = false, message = "Phòng bận! Vui lòng chọn thời gian hoặc phòng khác" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, message = "Phòng rảnh!Mời bạn tiếp tục!" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DangKyDichVu(int? MaDatPhong)
        {
            ViewBag.MaDatPhong = MaDatPhong;
            ViewBag.MaKhachHang = db.DatPhongs.FirstOrDefault(t => t.MaDatPhong == MaDatPhong).MaKhachHang;
            var ds = db.DichVus.ToList();
            return View(ds);
        }
        public ActionResult DsDichVu(int? MaDatPhong)
        {
            var ds = db.PhieuDichVus.Where(t => t.MaDatPhong == MaDatPhong).ToList();
            ViewBag.MaKhachHang = db.DatPhongs.FirstOrDefault(t => t.MaDatPhong == MaDatPhong).MaKhachHang;
            ViewBag.MaDatPhong = MaDatPhong;
            return View(ds);
        }
        public ActionResult Xoa(int? MaDatPhong,int? MaDichVu)
        {
            PhieuDichVu Phieu = db.PhieuDichVus.FirstOrDefault(t => t.MaDatPhong == MaDatPhong && t.MaDichVu==MaDichVu);
            db.PhieuDichVus.Remove(Phieu);
            db.SaveChanges();
            ViewBag.MaDatPhong = MaDatPhong;
            return RedirectToAction("DsDichVu",new { MaDatPhong = MaDatPhong });
        }
        [HttpPost]
        public ActionResult DangKyDV(int MaDatPhong,int? SoNguoi,int? MaDichVu)
        {
            PhieuDichVu p = new PhieuDichVu();
            p.MaDatPhong = MaDatPhong;
            p.NgayLap = DateTime.Now;
            p.TrangThai = "Chưa duyệt";
            if (SoNguoi == null)
                SoNguoi = 1;
            p.SoNguoi = SoNguoi;
            p.MaDichVu = MaDichVu;
            ViewBag.MaKhachHang = db.DatPhongs.FirstOrDefault(t => t.MaDatPhong == MaDatPhong).MaKhachHang;
            db.PhieuDichVus.Add(p);
            db.SaveChanges();
            return RedirectToAction("DangKyDichVu","TrangChu",new { MaDatPhong = MaDatPhong });
        }
        public ActionResult HuyPhong(int MaDatPhong)
        {
            DatPhong Dp = db.DatPhongs.FirstOrDefault(t => t.MaDatPhong == MaDatPhong);
            int? MaKhachHang = Dp.MaKhachHang;
            ViewBag.MaDatPhong = MaDatPhong;
            var dv = db.PhieuDichVus.Where(t => t.MaDatPhong == MaDatPhong);
            db.PhieuDichVus.RemoveRange(dv);
            db.DatPhongs.Remove(Dp);
            db.SaveChanges();    
            return RedirectToAction("PhongDaDat", new { MaKhachHang=MaKhachHang });
        }
        public ActionResult Loc(int MaKhachHang,decimal loai)
        {
            Session["MaKhachHang"] = MaKhachHang;
            var ds = db.Phongs.Where(t => t.GiaMoiDem < loai).ToList();
            return RedirectToAction("TrangChu",ds);
        }
        public ActionResult SuaThongTin(int? MaKhachHang)
        {

            //Lấy tên người dùng
            KhachHang kh = db.KhachHangs.FirstOrDefault(t => t.MaKhachHang == MaKhachHang);
            DateTime? ngaySinh = kh.ngaySinh;
            if (ngaySinh.HasValue)
            {
                string ngaySinhFormatted = ngaySinh.Value.ToString("dd/MM/yyyy");
                ViewBag.ngaySinh = ngaySinhFormatted;
            }
            else
            {
                ViewBag.ngaySinh = "Lỗi định dạng";
            }
            ViewBag.MaKhachHang = MaKhachHang;
            return View(kh);
        }
        [HttpPost]
        public ActionResult CapNhatThongTin(string tenND, string quocGia, string gioiTinh, DateTime? ngaySinh, string email, string SDT, string diaChi, int? MaKhachHang)
        {
            KhachHang nd = db.KhachHangs.FirstOrDefault(t => t.MaKhachHang == MaKhachHang);
            if (nd != null)
            {
                if (tenND != "")
                    nd.HoTen = tenND;
                if (quocGia != "")
                    nd.QuocGia = quocGia;
                if (gioiTinh != "")
                    nd.gioiTinh = gioiTinh;
                if (ngaySinh.HasValue)
                {
                    nd.ngaySinh = ngaySinh;
                }
                if (email != "")
                    nd.Email = email;
                if (SDT != "")
                    nd.SoDienThoai = SDT;
                if (diaChi != "")
                    nd.DiaChi = diaChi;
                db.SaveChanges();
            }
            return RedirectToAction("SuaThongTin", new { MaKhachHang = MaKhachHang });
        }
    }
} 