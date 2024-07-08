using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace StudentFucksTeacher
{
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
            driver.Navigate().GoToUrl("https://i.chaoxing.com/base/app?s=f08311a89d09fe39402171878faf40f0&fid=356&tid=44646");

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
            surveyElement.SendKeys("评价问卷");

            driver.FindElement(By.ClassName("search-btn")).Click();
            
            await Task.Delay(500);
            
            // 点击问卷按钮
            found = false;
            while (!found)
            {
                var surveyApplication = driver.FindElements(By.XPath("/html/body/div/div/div/div[2]/div[8]/ul/li[2]"));
                found = surveyApplication.Count > 0;
                await Task.Delay(1000);
                await Console.Out.WriteLineAsync("Still finding app...");
            }
            
            // Find the first survey element and click it
            var buttonElement = driver.FindElement(By.XPath("/html/body/div/div/div/div[2]/div[8]/ul/li[2]"));
            
            buttonElement.Click();
            
            // 在点击之前，获取当前窗口的句柄
            var currentWindowHandle = driver.CurrentWindowHandle;

            // 执行点击操作，这将打开新的窗口
            surveyElement.Click();

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
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            IWebElement surveyElement = wait.Until(driver =>
            {
                var elements = driver.FindElements(By.XPath("//a[text()='待评价']"));
                return elements.Count > 0 ? elements[0] : null;
            });

            if (surveyElement != null)
            {
                surveyElement.Click();
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
            while (true)
            {
                // Find all radio button elements
                var inputs = driver.FindElements(By.TagName("input"));

                if (inputs.Count == 0)
                {
                    await Console.Out.WriteLineAsync("No radio buttons found... Quitting...");
                    break;
                }

                // Filter out the radio buttons
                var radioButtons = inputs.Where(input => input.GetAttribute("type") == "radio").ToList();

                foreach (var radioButton in radioButtons)
                {
                    var parent = radioButton.FindElement(By.XPath("..")); // Find the parent element

                    
                    if (parent.Text.Contains("5")) // Check if the parent's text contains "5"
                    {
                        radioButton.Click(); // Click the radio button if the condition is met
                    }
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
                ((IJavaScriptExecutor) driver)
                    .ExecuteScript("window.scrollTo(0,0)");

                // Find the next button
                var nextButton = driver.FindElement(By.ClassName("layui-layer-btn0"));

                // Create a WebDriverWait instance
                WebDriverWait waitButton = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                // Wait until the next button is clickable
                waitButton.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(nextButton));

                await Console.Out.WriteLineAsync($"Next... {nextButton.Text}");

                // Click the next button
                nextButton.Click();
                
                // Wait for 5 seconds before continuing
                await Task.Delay(1000);

                await Console.Out.WriteLineAsync("Done!");
            }
        }
    }
}