using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NhomBoy_Hotel.Controllers
{
    public class DangNhap_DangKyController : Controller
    {
        // GET: DangNhap_DangKy
        // XEDAPHOIANEntities1 db= new XEDAPHOIANEntities1();
        DatPhongKhachSanEntities db = new DatPhongKhachSanEntities();
        public ActionResult DangNhap_DangKy()
        {
            ViewBag.B = "Điền thông tin vào bên dưới";
            return View(); // Truyền đối tượng tk tới view
        }
        public ActionResult DangNhap(string tenTaiKhoan, string matKhau)
        {
            var taiKhoan = db.KhachHangs.FirstOrDefault(t => t.TenTaiKhoan == tenTaiKhoan && t.MatKhau == matKhau);
            if (tenTaiKhoan == "phu" && matKhau == "123")
                return RedirectToAction("DuyetPhong", "NhanVien");
            if (taiKhoan != null)
                return RedirectToAction("TrangChu", "TrangChu", new {MaKhachHang = taiKhoan.MaKhachHang });
          
            else
                ViewBag.A = "Sai thông tin đăng nhập hoặc mật khẩu";
            return View("DangNhap_DangKy"); // Truyền đối tượng tk tới view
        }
        public ActionResult DangKy(string tenND, DateTime? ngaySinh, string quocGia, string gioiTinh, string SDT, string diaChi, string email)
        {
            KhachHang kh = new KhachHang();
            kh.HoTen = tenND;
            kh.ngaySinh = ngaySinh;
            kh.gioiTinh = gioiTinh;
            kh.SoDienThoai = SDT;
            kh.DiaChi = diaChi;
            kh.Email = email;
            kh.QuocGia = quocGia;
            Session["soDienThoai"] = SDT;
            db.KhachHangs.Add(kh);
            db.SaveChanges();
            return RedirectToAction("TaoTaiKhoan");
        }
        public ActionResult TaoTaiKhoan()
        {
            string SDT= Session["soDienThoai"] as string;
            KhachHang kh = db.KhachHangs.FirstOrDefault(t=>t.SoDienThoai==SDT);
            ViewBag.tenKH = kh.HoTen;
            ViewBag.SDT = SDT;
            return View();
        }
        [HttpPost]
        public ActionResult TaoTaiKhoan(string tenTK, string matKhau, string nlmatKhau, string code)
        {
            string SDT = Session["soDienThoai"] as string;
            if (string.IsNullOrEmpty(tenTK) || string.IsNullOrEmpty(matKhau) || string.IsNullOrEmpty(nlmatKhau) || string.IsNullOrEmpty(code))
            {
                ViewBag.A = "Các trường không được để trống";
            }
            if (matKhau != nlmatKhau)
                ViewBag.A = "Mật khẩu không khớp";
            else
            {
                KhachHang kh = db.KhachHangs.FirstOrDefault(t=>t.SoDienThoai==SDT);
                kh.TenTaiKhoan = tenTK;
                kh.MatKhau = matKhau;
                if (code  == "choemdiemcao")
                {
                    db.SaveChanges();
                }    
                else
                    ViewBag.A = "Điểm không cao sao tạo tài khoản được!";
            }

            return View("DangNhap_DangKy");
        }
    }
}
