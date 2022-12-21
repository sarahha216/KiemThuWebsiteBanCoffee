using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace LoaiSP_SanPham
{
    [TestClass]
    public class LoaiSP_SP
    {
        int waitingTime = 1000;
        By taiKhoan;
        By matKhau;
        By loginButton;
        IWebDriver webDriver;

        int countLoai = 0;
        List<Tuple<string, string, string>> list;

        public void DangNhap()
        {
            taiKhoan = By.Name("SDT");
            matKhau = By.Name("MATKHAU");
            loginButton = By.Name("BtnSubmit");

            webDriver = new ChromeDriver();
            Thread.Sleep(waitingTime);
            webDriver.Navigate().GoToUrl("http://coffeeshop.somee.com/Authentication/Login");
            Thread.Sleep(waitingTime);
            webDriver.Manage().Window.Maximize();

            //đăng nhập
            webDriver.FindElement(taiKhoan).SendKeys("9876543210");
            Thread.Sleep(waitingTime);
            webDriver.FindElement(matKhau).SendKeys("123456");
            Thread.Sleep(waitingTime);

            var btnLogin = webDriver.FindElement(loginButton);
            btnLogin.Click();
            Thread.Sleep(waitingTime);
        }
        public void GoTo_LoaiSP()
        {
            //chuyển đến link loại sản phẩm
            var sideBar = webDriver.FindElement(By.CssSelector("[href*='/LoaiSanPham']"));
            sideBar.Click();
            Thread.Sleep(waitingTime);
        }
        public void GoTo_SanPham()
        {
            //chuyển đến link sản phẩm
            var sideBar = webDriver.FindElement(By.CssSelector("[href*='/SanPham']"));
            sideBar.Click();
            Thread.Sleep(waitingTime);
        }
        public void GoTo_ThemLoaiSP()
        {
            //chuyển đến thêm loại sản phẩm
            var btnCreate = webDriver.FindElement(By.CssSelector("[href*='/LoaiSanPham/Create']"));
            btnCreate.Click();
            Thread.Sleep(waitingTime);
        }
        public void GoTo_ThemSanPham()
        {
            //chuyển đến thêm sản phẩm
            var btnCreate = webDriver.FindElement(By.CssSelector("[href*='/SanPham/Create']"));
            btnCreate.Click();
            Thread.Sleep(waitingTime);
        }
        public void DemLoai()
        {
            //lấy dữ liệu loại sản phẩm và đếm
            IList<IWebElement> data = webDriver.FindElements(By.XPath(".//li[contains(@class, 'product-type__item')]"));
            Thread.Sleep(waitingTime);

            countLoai = data.Count();
        }
        public void KetThuc()
        {
            webDriver.Quit();
        }
        public void Get_List_SanPham()
        {
            IList<IWebElement> data = webDriver.FindElements(By.XPath(".//li[contains(@class, 'product__item')]"));
            Thread.Sleep(waitingTime);

            list = new List<Tuple<string, string, string>>();

            //lấy data
            foreach (IWebElement item in data)
            {
                try
                {
                    var name = item.FindElement(By.XPath(".//div[contains(@class, 'product__item__title')]")).Text;
                    var price = item.FindElement(By.XPath(".//div[contains(@class, 'product__item__price')]")).Text;
                    var img = item.FindElement(By.XPath(".//img[contains(@class, 'product__item__img')]")).GetAttribute("src");
                    list.Add(new Tuple<string, string, string>(name.Trim(), price, img));
                }
                catch (NoSuchElementException)
                {

                }
            }
        }
        public int DemSanPham()
        {
            IList<IWebElement> data = webDriver.FindElements(By.XPath(".//li[contains(@class, 'product__item')]"));
            Thread.Sleep(waitingTime);
            return data.Count();
        }
        public void ChonAnh()
        {
            var value = "coffeehoatan.jpg";
            string filePath = string.Format(@"C:\Users\AD\Downloads\111\{0}", value);
            Thread.Sleep(waitingTime);
            try
            {
                webDriver.FindElement(By.XPath("//input[@type='file']")).SendKeys(filePath);
                Console.WriteLine("Chọn ảnh Passed");
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Chọn ảnh Failed");
            }
            Thread.Sleep(waitingTime);
        }
        [TestMethod]
        public void TraCuuSanPham_01()
        {
            DangNhap();

            IList<IWebElement> data = webDriver.FindElements(By.XPath(".//li[contains(@class, 'product__item')]"));
            Thread.Sleep(waitingTime);

            int expected = data.Count();
            Console.WriteLine("Expected: " + expected);

            webDriver.FindElement(By.Id("SearchString")).SendKeys("");
            webDriver.FindElement(By.XPath("//input[@type='submit']")).Click();
            Get_List_SanPham();
            int actual = list.Count();
            Console.WriteLine("Actual: " + actual);
            Assert.AreEqual(expected, actual);
            
            KetThuc();
        }
        [TestMethod]
        public void TraCuuSanPham_02()
        {
            DangNhap();
            webDriver.FindElement(By.Id("SearchString")).SendKeys("Cà phê");
            webDriver.FindElement(By.XPath("//input[@type='submit']")).Click();
            
            Get_List_SanPham();
            
            int soLuong = list.Count();
            Console.WriteLine("Số lượng: " + soLuong);

            string expected_1 = "Không tìm thấy";
            string actual_1 = "";

            bool check = true;

            if (soLuong == 0)
            {
                KetThuc();
                Assert.AreEqual(expected_1, actual_1);
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (!Regex.Match(list[i].Item1, "Cà Phê").Success)
                    {
                        check = false;
                        break;
                    }
                    else
                    {
                        check = true;
                    }
                }
                if (check == false)
                {
                    Console.WriteLine("Xuất hiện 1 item không có chữ Cà Phê");
                }
                else
                {
                    Console.WriteLine("Danh sách sản phẩm: ");
                    for (int i = 0; i < list.Count; i++)
                    {
                        Console.WriteLine("Tên sản phẩm [{0}]: {1} ", i, list[i].Item1);
                    }
                }
            }
            KetThuc();
        }
        [TestMethod]
        public void ThemLoaiSP()
        {
            DangNhap();
            GoTo_LoaiSP();
            GoTo_ThemLoaiSP();

            //thêm loại
            webDriver.FindElement(By.Name("TENLOAISP")).SendKeys("Test");

            var submitButton = webDriver.FindElement(By.CssSelector(".btn-default[value='Thêm']"));
            submitButton.Click();
            Thread.Sleep(waitingTime);

            //lấy data list
            IList<IWebElement> data = webDriver.FindElements(By.XPath(".//li[contains(@class, 'product-type__item')]"));
            Thread.Sleep(waitingTime);

            List<string> nameList = new List<string>();
            string actual = "";
            string expected = "Thêm thành công";
            string thongbaoGhi = "Chưa thêm vào database";
            int count = 0;

            //lấy data
            foreach (IWebElement item in data)
            {
                try
                {
                    var name = item.FindElement(By.XPath(".//span[contains(@class, 'product-type__name')]")).Text;
                    nameList.Add(name);
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("Failed");
                }
            }
            //kiểm tra data
            for (int i = 0; i < nameList.Count; i++)
            {
                if (nameList[i].Equals("Test"))
                {
                    thongbaoGhi = "Đã thêm vào database";
                    actual = "Không thông báo";
                    count += 1;
                }
            }
            Console.WriteLine(thongbaoGhi);

            //kiểm tra data có trùng?
            if (count > 1)
            {
                Console.WriteLine("Trùng dữ liệu");
            }

            //so sánh
            KetThuc();
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void ThongKe_LoaiSP()
        {
            DangNhap();
            GoTo_LoaiSP();
            DemLoai();

            int expected = countLoai;
            Console.WriteLine("Expected: " + expected);

            var x = webDriver.FindElement(By.XPath("//div[@class='product--top']//p[2]"));
            string s = x.Text.Remove(0, 17).Trim();
            int actual = int.Parse(s);
            Console.WriteLine("Actual: " + actual);
            Assert.AreEqual(expected, actual);
            KetThuc();
        }
        
        [TestMethod]
        public void DanhSachSP_TheoLoai()
        {
            DangNhap();
            GoTo_LoaiSP();

            var index = webDriver.FindElement(By.XPath(".//li[contains(@class, 'product-type__item')]"));
            index.Click();
            Thread.Sleep(waitingTime);

            var item = webDriver.FindElement(By.XPath("(.//li[contains(@class, 'product__item')])[1]"));
            item.Click();
            Thread.Sleep(waitingTime);

            string actual = "";
            string expected = "Cà phê";
            Console.WriteLine("Expected: " + expected);
            try
            {
                var tenLoai = webDriver.FindElement(By.XPath(".//div[@class='details-right']/div[3]"));
                Console.WriteLine("Actual: " + tenLoai.Text);
                actual = tenLoai.Text;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Failed");
            }
            KetThuc();
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void TestChonAnh()
        {
            DangNhap();
            GoTo_ThemSanPham();
            var value = "coffeehoatan.jpg";
            string filePath = string.Format(@"C:\Users\AD\Downloads\111\{0}", value);
            Thread.Sleep(waitingTime);
            try
            {
                webDriver.FindElement(By.XPath("//input[@type='file']")).SendKeys(filePath);
                Console.WriteLine("Chọn ảnh Passed");
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Chọn ảnh Failed");
            }
            Thread.Sleep(waitingTime);
            KetThuc();
        }
        [TestMethod]
        public void DropDownButton_LoaiSP()
        {
            DangNhap();
            GoTo_LoaiSP();
            DemLoai();
            Console.WriteLine("Expected: " + countLoai);
            GoTo_SanPham();

            GoTo_ThemSanPham();
            IWebElement element = webDriver.FindElement(By.XPath("//*[@id='MALOAISP']"));
            IList<IWebElement> data = element.FindElements(By.XPath("//*[@id='MALOAISP']//option"));
            int count = data.Count();
            Console.WriteLine("Actual: " + count);

            KetThuc();
            Assert.AreEqual(countLoai, count);
        }
        [TestMethod]
        public void DropDownButton_CongThuc()
        {
            DangNhap();
            //chuyển đến link công thức
            var sideBar = webDriver.FindElement(By.CssSelector("[href*='/CongThuc']"));
            sideBar.Click();
            Thread.Sleep(waitingTime);

            //lấy data list
            IList<IWebElement> data = webDriver.FindElements(By.XPath(".//li[contains(@class, 'product-type__item')]"));
            Thread.Sleep(waitingTime);
            Console.WriteLine("Expected: " + data.Count);

            GoTo_SanPham();
            GoTo_ThemSanPham();
            IWebElement element = webDriver.FindElement(By.XPath("//*[@id='recipe']"));
            IList<IWebElement> data2 = element.FindElements(By.XPath("//*[@id='recipe']//option"));
            int count = data.Count();
            Console.WriteLine("Actual: " + count);

            KetThuc();
        }
        [TestMethod]
        public void ThemSanPham_01()
        {
            DangNhap();
            GoTo_ThemSanPham();
            //thêm sản phẩm
            webDriver.FindElement(By.Id("TENSP")).SendKeys("Test");
            webDriver.FindElement(By.Id("GIASP")).SendKeys("70000");
            webDriver.FindElement(By.Id("MOTASP")).SendKeys("0");
            ChonAnh();
            var submitButton = webDriver.FindElement(By.CssSelector(".btn-default[value='Thêm']"));
            submitButton.Click();
            Thread.Sleep(waitingTime);

            GoTo_LoaiSP();
            var index = webDriver.FindElement(By.XPath("(//li[contains(@class, 'product-type__item')])[1]"));
            index.Click();
            Thread.Sleep(waitingTime);

            Get_List_SanPham();
            CheckSanPham_SauKhiThem_01();
        }
        public void CheckSanPham_SauKhiThem_01()
        {
            string actual = "";
            string expected = "Thêm thành công";
            int count = 0;

            //xử lý
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].Item1.Equals("Test") && list[i].Item2.Equals("70000") && !String.IsNullOrEmpty(list[i].Item3))
                {
                    Console.WriteLine("Đã ghi vào database");
                    actual = "Không thông báo";
                }
            }
            //kiểm tra xem tên có bị trùng?
            for (int i = 0; i < list.ToList().Count; i++)
            {
                if (list[i].Item1.Equals("Test"))
                {
                    count += 1;
                }
            }
            
            if (count > 1)
            {
                Console.WriteLine("Tên bị trùng");
            }
            //so sánh
            KetThuc();
            Assert.AreEqual(expected, actual);
        }
        
        //thêm sản phẩm với ảnh là file word
        [TestMethod]
        public void ThemSanPham_02()
        {
            DangNhap();
            GoTo_ThemSanPham();
            //thêm sản phẩm
            webDriver.FindElement(By.Id("TENSP")).SendKeys("Test 1");
            webDriver.FindElement(By.Id("GIASP")).SendKeys("70000");
            webDriver.FindElement(By.Id("MOTASP")).SendKeys("0");

            //chọn ảnh là file word
            var value = "word.docx";
            string filePath = string.Format(@"C:\Users\AD\Downloads\111\{0}", value);
            Thread.Sleep(waitingTime);
            try
            {
                webDriver.FindElement(By.XPath("//input[@type='file']")).SendKeys(filePath);
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Lỗi");
            }
            Thread.Sleep(waitingTime);
            
            var submitButton = webDriver.FindElement(By.CssSelector(".btn-default[value='Thêm']"));
            submitButton.Click();
            Thread.Sleep(waitingTime);

            var error = webDriver.FindElement(By.XPath("//h2[text()='An error occurred while processing your request.']"));
            string expected = "Vui lòng kiểm tra thông tin";
            string actual = error.Text;
            KetThuc();
            Assert.AreEqual(expected, actual);
        }

        //nhấn 2 lần button thêm xem có duplicated?
        [TestMethod]
        public void ThemSanPham_03()
        {
            DangNhap();
            GoTo_ThemSanPham();
            //thêm sản phẩm
            webDriver.FindElement(By.Id("TENSP")).SendKeys("Double click 1");
            webDriver.FindElement(By.Id("GIASP")).SendKeys("30000");
            webDriver.FindElement(By.Id("MOTASP")).SendKeys("Double click 1");
            ChonAnh();
            Thread.Sleep(waitingTime);
            try
            {
                Actions a = new Actions(webDriver);
                a.MoveToElement(webDriver.FindElement(By.XPath("//input[@type='submit']"))).DoubleClick().Build().Perform();
                Thread.Sleep(waitingTime);

                GoTo_LoaiSP();
                var index = webDriver.FindElement(By.XPath("(//li[contains(@class, 'product-type__item')])[1]"));
                index.Click();
                Thread.Sleep(waitingTime);

                Get_List_SanPham();
                CheckSanPham_SauKhiThem_03();
                Thread.Sleep(waitingTime);
                KetThuc();
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Failed");
                KetThuc();
            }
        }
        public void CheckSanPham_SauKhiThem_03()
        {
            string actual = "";
            string expected = "Thêm thành công";
            int count = 0;
            
            //xử lý
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].Item1.Equals("Double click 1") && list[i].Item2.Equals("30000") && !String.IsNullOrEmpty(list[i].Item3))
                {
                    Console.WriteLine("Đã ghi vào database");
                    actual = "Không thông báo";
                }
            }
            //kiểm tra xem tên có bị trùng?
            for (int i = 0; i < list.ToList().Count; i++)
            {
                if (list[i].Item1.Equals("Double click 1"))
                {
                    count += 1;
                }
            }

            if (count > 1)
            {
                Console.WriteLine("Tên bị trùng");
            }
            //so sánh
            KetThuc();
            Assert.AreEqual(expected, actual);
        }

        //công thức khác loại sản phẩm
        public void ThemSanPham_04()
        {
            
        }
        [TestMethod]
        public void XoaSanPham()
        {
            DangNhap();
            Console.WriteLine("SL sản phẩm trước khi xóa: " + DemSanPham());
            //thực hiện xóa
            var href = webDriver.FindElement(By.XPath("(//li[contains(@class, 'product__item')])[43]/div/a[2]")).GetAttribute("href");
            Console.WriteLine(href);
            Thread.Sleep(waitingTime);
            webDriver.Navigate().GoToUrl(href);

            var confirm = webDriver.FindElement(By.XPath("//input[@type='submit']"));
            confirm.Click();
            Thread.Sleep(waitingTime);
            Console.WriteLine("Đã xóa");

            //kiểm tra dữ liệu
            string expected = "Xóa thành công";
            string actual = "Không thông báo";
            Console.WriteLine("SL sản phẩm sau khi xóa: " + DemSanPham());

            KetThuc();
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void ChinhSuaSanPham()
        {
            DangNhap();
            var href = webDriver.FindElement(By.XPath("(//li[contains(@class, 'product__item')])[37]/div/a[1]")).GetAttribute("href");
            Console.WriteLine(href);
            Thread.Sleep(waitingTime);
            webDriver.Navigate().GoToUrl(href);

            //sửa sản phẩm
            webDriver.FindElement(By.Id("TENSP")).Clear();
            webDriver.FindElement(By.Id("TENSP")).SendKeys("Test 4");
            webDriver.FindElement(By.Id("GIASP")).Clear();
            webDriver.FindElement(By.Id("GIASP")).SendKeys("70000");

            var submitButton = webDriver.FindElement(By.CssSelector(".btn-default[value='Cập nhật']"));
            submitButton.Click();
            Thread.Sleep(waitingTime);

            GoTo_LoaiSP();
            var index = webDriver.FindElement(By.XPath("(//li[contains(@class, 'product-type__item')])[1]"));
            index.Click();
            Thread.Sleep(waitingTime);

            Get_List_SanPham();
            string actual = "";
            string expected = "Sửa thành công";


            //xử lý
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].Item1.Equals("Test 4") && list[i].Item2.Equals("70000") && !String.IsNullOrEmpty(list[i].Item3))
                {
                    Console.WriteLine("Đã ghi vào database");
                    actual = "Không thông báo";
                }
            }
            //so sánh
            KetThuc();
            Assert.AreEqual(expected, actual);
        }

        //public void Test_DoubleClick()
        //{
        //    IWebDriver driver = new ChromeDriver();
        //    String url = "http://www.uitestpractice.com/Students/Actions";
        //    driver.Navigate().GoToUrl(url);
        //    Thread.Sleep(waitingTime);
        //    driver.Manage().Window.Maximize();
        //    Thread.Sleep(waitingTime);
        //    try
        //    {
        //        Actions a = new Actions(driver);
        //        a.MoveToElement(driver.FindElement(By.Name("dblClick"))).DoubleClick().Build().Perform();
        //        Thread.Sleep(waitingTime);
        //    }
        //    catch
        //    {

        //    }
        //}
    }
}
