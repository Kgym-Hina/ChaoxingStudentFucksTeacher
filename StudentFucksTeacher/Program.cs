using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace StudentFucksTeacher
{
    // 测试
    internal static class Program
    {
        private static async Task Main()
        {
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://i.chaoxing.com/");

            await CheckUrlChange(driver);
        }

        private static async Task CheckUrlChange(IWebDriver driver)
        {
            var currentUrl = driver.Url;

            while (true)
            {
                await Task.Delay(1000); // Wait for 1 second
                await Console.Out.WriteLineAsync("Still fiding user...");
                if (driver.Url != currentUrl)
                {
                    Console.WriteLine("Logged In");
                    currentUrl = driver.Url;

                    await CheckInbox(driver);
                    break;
                }
            }
        }

        private static async Task CheckInbox(IWebDriver driver)
        {
            // 搜索应用中心
            driver.Navigate()
                .GoToUrl("https://i.chaoxing.com/base/app?s=f08311a89d09fe39402171878faf40f0&fid=356&tid=44646");

            var found = false;
            while (!found)
            {
                var surveyApplication = driver.FindElements(By.Id("inputAppName"));
                found = surveyApplication.Count > 0;
                await Task.Delay(1000);
                await Console.Out.WriteLineAsync("Still finding app...");
            }

            // Find the first survey element and click it
            var surveyElement = driver.FindElement(By.Id("inputAppName"));

            surveyElement.Click();
            surveyElement.SendKeys("学生评价");

            driver.FindElement(By.ClassName("search-btn")).Click();

            await Task.Delay(500);

            // 点击问卷按钮
            found = false;
            while (!found)
            {
                var surveyApplication = driver.FindElements(By.CssSelector("[title='学生评价']"));
                found = surveyApplication.Count > 0;
                await Task.Delay(1000);
                await Console.Out.WriteLineAsync("Still finding app...");
            }

            // Find the first survey element and click it
            var buttonElement = driver.FindElement(By.CssSelector("[title='学生评价']"));

            // 在点击之前，获取当前窗口的句柄
            var currentWindowHandle = driver.CurrentWindowHandle;

            // 执行点击操作，这将打开新的窗口
            buttonElement.Click();

            // 获取所有窗口的句柄
            var windowHandles = driver.WindowHandles;

            // 切换到新打开的窗口
            foreach (var handle in windowHandles)
            {
                if (handle != currentWindowHandle)
                {
                    driver.SwitchTo().Window(handle);
                    break;
                }
            }

            //
            await ChooseCourse(driver);
        }

        private static async Task ChooseCourse(IWebDriver driver)
        {
            driver.Navigate().Refresh();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            IWebElement surveyElement = wait.Until(driver =>
            {
                var elements = driver.FindElements(By.XPath("//i[@class='d_submit_tag' and text()='未完成']"));
                return elements.Count > 0 ? elements[0] : null;
            });

            if (surveyElement != null)
            {
                // Find the parent row of the located element
                var parentRow = surveyElement.FindElement(By.XPath("./ancestor::tr"));

                // Find the span element within the same row
                var spanElement = parentRow.FindElement(By.XPath(".//span[text()='查看详情']"));

                // Click the span element to view details
                spanElement.Click();

                // Wait for 5 seconds before continuing
                await Task.Delay(5000);

                await CompleteForm(driver);
                Console.ReadLine();
            }
            else
            {
                await Console.Out.WriteLineAsync("Element not found...");
            }
        }

        private static async Task CompleteForm(IWebDriver driver)
        {
            try
            {
                while (true)
                {
                    // Refresh the page
                    driver.Navigate().Refresh();

                    WebDriverWait wait1 = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                    wait1.Until(
                        SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//span[text()='评价状态']")));

                    Console.WriteLine("评价状态");
                    // Find span 评价状态
                    var statusElement = driver.FindElement(By.XPath("//span[text()='评价状态']"));
                    statusElement.Click();

                    await Task.Delay(800);

                    Console.WriteLine("未评");
                    // Find <li label="0,未评" class="el-table-filter__list-item">未评</li>
                    var notEvaluatedElement = driver.FindElement(By.XPath("//li[text()='未评']"));
                    notEvaluatedElement.Click();

                    await Task.Delay(800);

                    Console.WriteLine("寻找 评价");
                    // Wait Until the page is loaded
                    WebDriverWait wait2 = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    wait2.Until(
                        SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.XPath("//a[text()='评价']")));

                    Console.WriteLine("评价");
                    // Click Start
                    var evaluateLink = driver.FindElement(By.XPath("//a[text()='评价']"));
                    evaluateLink.Click();

                    await Task.Delay(500);

                    // Find all input elements that are not hidden and of type text
                    var inputs = driver.FindElements(By.XPath("//input[@type='text' and not(@type='hidden')]"));

                    if (inputs.Count == 0)
                    {
                        await Console.Out.WriteLineAsync("No text inputs found... Quitting...");
                        break;
                    }

                    foreach (var input in inputs)
                    {
                        input.Clear(); // Clear the input field
                        input.SendKeys("20"); // Fill the input field with the value "20"
                    }

                    // Fill the text area with classname blueTextarea
                    var textArea = driver.FindElement(By.ClassName("blueTextarea"));
                    textArea.SendKeys("还行");

                    // Find the submit button within the container with classname 'botBtnBox' and click it
                    var submitButton = driver.FindElement(By.XPath("//div[@class='botBtnBox']//a[text()='提交']"));
                    await Console.Out.WriteLineAsync($"Submitting... {submitButton.Text}");
                    submitButton.Click();

                    await Task.Delay(500);

                    // Scroll to the bottom of the page
                    ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0,0)");

                    // Find the next button
                    var nextButton = driver.FindElement(By.ClassName("layui-layer-btn0"));

                    // Create a WebDriverWait instance
                    WebDriverWait waitButton = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                    // Wait until the next button is clickable
                    waitButton.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(nextButton));

                    await Console.Out.WriteLineAsync($"Next... {nextButton.Text}");

                    // Click the next button
                    nextButton.Click();

                    // Wait for 1 second before continuing
                    await Task.Delay(1000);

                    await Console.Out.WriteLineAsync("Done!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("没了，返回上一级");
                driver.Navigate().Back();
                await ChooseCourse(driver);
            }
        }
    }
}