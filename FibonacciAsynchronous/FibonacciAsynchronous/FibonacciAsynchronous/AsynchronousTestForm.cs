//Josh Depauw
// Fig. 23.3: AsynchronousTestForm.cs
// Fibonacci calculations performed in separate threads
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FibonacciAsynchronous
{
    public partial class AsynchronousTestForm : Form
    {
        public AsynchronousTestForm()
        {
            InitializeComponent();
        }

        // start asynchronous calls to Fibonacci
        private async void startButton_Click(object sender, EventArgs e)
        {
            try
            {
                int fib = int.Parse(txtInput.Text); //converts user input into an int
                int fibPlusOne = fib + 1; //fibonicci (x + 1)
                int fibPlusTwo = fib + 2; // fibonicci (x + 2)
               

                //validate input is between 1-100
                if (fib < 1 || fib > 100)
                {
                    MessageBox.Show("Number must be between 1-100!");
                    txtInput.Clear();
                }
                else
                {

                    outputTextBox.Text =
                       $"Starting Task to calculate Fibonacci({fib})\r\n";

                    // create Task to perform Fibonacci(user input) calculation in a thread
                    Task<TimeData> task1 = Task.Run(() => StartFibonacci(fib));

                    outputTextBox.AppendText(
                       $"Starting Task to calculate Fibonacci({fibPlusOne})\r\n");

                    // create Task to perform Fibonacci(input plus 1) calculation in a thread
                    Task<TimeData> task2 = Task.Run(() => StartFibonacci(fibPlusOne));

                    outputTextBox.AppendText(
                     $"Starting Task to calculate Fibonacci({fibPlusTwo})\r\n");

                    // create Task to perform Fibonacci(input plus 1) calculation in a thread
                    Task<TimeData> task3 = Task.Run(() => StartFibonacci(fibPlusTwo));

                    var tasks = new Task[]
                 {
                     task1, task2, task3
                 };

                    await Task.WhenAll(tasks); // wait for all to complete

                    // determine time that first thread started
                    DateTime startTime =
                       (task1.Result.StartTime < task2.Result.StartTime && task2.Result.StartTime < task3.Result.StartTime) ?
                       task1.Result.StartTime : task3.Result.StartTime;

                    // determine time that last thread ended
                    DateTime endTime =
                       (task1.Result.EndTime > task2.Result.EndTime && task2.Result.EndTime > task3.Result.EndTime) ?
                       task1.Result.EndTime : task3.Result.EndTime;

                    // display total time for calculations
                    double totalMinutes = (endTime - startTime).TotalMinutes;
                    outputTextBox.AppendText(
                       $"Total calculation time = {totalMinutes:F6} minutes\r\n");
                }
            }
            catch (FormatException) //exception handling if the user enters letters
            {
                MessageBox.Show("Numbers only please!");
                txtInput.Clear();
            }
            // starts a call to fibonacci and captures start/end times
            TimeData StartFibonacci(int n)
            {
                // create a TimeData object to store start/end times
                var result = new TimeData();

                AppendText($"Calculating Fibonacci({n})");
                result.StartTime = DateTime.Now;
                long fibonacciValue = Fibonacci(n);
                result.EndTime = DateTime.Now;

                double minutes =
                   (result.EndTime - result.StartTime).TotalMinutes;
                AppendText($"Fibonacci{n}) = {fibonacciValue} \r\n Calculation time = {minutes:F6} minutes\r\n");

                return result;
            }
        }


            // Recursively calculates Fibonacci numbers
            public long Fibonacci(long n)
            {
                if (n == 0 || n == 1)
                {
                    return n;
                }
                else
                {
                    return Fibonacci(n - 1) + Fibonacci(n - 2);
                }
            }

            // append text to outputTextBox in UI thread
            public void AppendText(String text)
            {
                if (InvokeRequired) // not GUI thread, so add to GUI thread
                {
                    Invoke(new MethodInvoker(() => AppendText(text)));
                }
                else // GUI thread so append text
                {
                    outputTextBox.AppendText(text + "\r\n");
                }
            }
        }
    }