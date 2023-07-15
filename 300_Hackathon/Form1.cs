using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace _300_Hackathon
{
    public partial class omer : Form
    {
        private SemaphoreSlim empty = new SemaphoreSlim(12, 12); // Initialized with 12 empty slots
        private SemaphoreSlim full = new SemaphoreSlim(0, 12); // Initialized with 0 filled slots
        private Mutex mutex = new Mutex();
        private int producerRate;
        private int consumerRate;
        private int numberProducers;
        private int numberConsumers;
        private List<Thread> runThreads = new List<Thread>();
        private List<PictureBox> pictureBoxes = new List<PictureBox>(); // Store the picture boxes
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private int waiting = 0;
        private double totalWaitingTime = 0;
        private int numberOfWaits = 0;
        private DateTime start;

        public omer()
        {
            InitializeComponent();

            textBox1 = fight2.GetText1();

            textBox2 = fight2.GetText2();

            textBox3 = fight2.GetText3();
            textBox4 = fight2.GetText4();
            textBox2.TextChanged += TextBox2_TextChanged;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numberProducers = (int)numericUpDown1.Value;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            InitializePictureBoxes();
            InitializeThreads();

            bunifuTransition2.ShowSync(loading_warriors1);
            await Task.Delay(6000);
            bunifuTransition1.ShowSync(fight2);
            bunifuTransition2.HideSync(loading_warriors1);
            start = DateTime.Now;

            StartThreads();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            numberConsumers = (int)numericUpDown3.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            producerRate = (int)numericUpDown2.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            consumerRate = (int)numericUpDown4.Value;
        }

        private void InitializePictureBoxes()
        {
            pictureBoxes.Add(pictureBox1);
            pictureBoxes.Add(pictureBox2);
            pictureBoxes.Add(pictureBox3);
            pictureBoxes.Add(pictureBox4);
            pictureBoxes.Add(pictureBox5);
            pictureBoxes.Add(pictureBox6);
            pictureBoxes.Add(pictureBox7);
            pictureBoxes.Add(pictureBox8);
            pictureBoxes.Add(pictureBox9);
            pictureBoxes.Add(pictureBox10);
            pictureBoxes.Add(pictureBox11);
            pictureBoxes.Add(pictureBox12);
        }

        private void InitializeThreads()
        {
            for (int i = 0; i < numberProducers; i++)
            {
                Thread producerThread = new Thread(ProducerThread);
                runThreads.Add(producerThread);
                
            }
            for (int i = 0; i < numberConsumers; i++)
            {
                Thread consumerThread = new Thread(ConsumerThread);
                runThreads.Add(consumerThread);

            }
        }

        private void StartThreads()
        {
            foreach (Thread thread in runThreads)
            {
                thread.Start();
            }
        }

        private void ProducerThread()
        {
            while (true)
            {
                Random rand = new Random();
                int randomPosition = rand.Next(maxValue: 12); // Generate a random position in the buffer
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Interlocked.Increment(ref numberOfWaits);
                Interlocked.Increment(ref waiting);
                stopwatch.Start();
                empty.Wait(); // Wait for an empty slot
                mutex.WaitOne(); // Acquire the mutex
                stopwatch.Stop();
                Interlocked.Decrement(ref waiting);
                totalWaitingTime += stopwatch.Elapsed.Milliseconds;
                if (!pictureBoxes[randomPosition].Visible)
                {
                    pictureBoxes[randomPosition].Invoke((Action)(() => pictureBoxes[randomPosition].Show()));
                    Thread.Sleep(300);
                }

                mutex.ReleaseMutex(); // Release the mutex
                full.Release(); // Signal that a slot is filled

                Thread.Sleep(producerRate * 1000);
            }
        }

        private void ConsumerThread()
        {
            while (true)
            {
                Stopwatch stopwatch = new Stopwatch();
                Random rand = new Random();
                int randomPosition = rand.Next(maxValue: 12); // Generate a random position in the buffer
                stopwatch.Start();
                Interlocked.Increment(ref numberOfWaits);
                Interlocked.Increment(ref waiting);
                full.Wait(); // Wait for a filled slot
                mutex.WaitOne(); // Acquire the mutex
                stopwatch.Stop();
                Interlocked.Decrement(ref waiting);
                totalWaitingTime += stopwatch.Elapsed.Milliseconds;
                if (pictureBoxes[randomPosition].Visible)
                {
                    pictureBoxes[randomPosition].Invoke((Action)(() => pictureBoxes[randomPosition].Hide()));
                    Thread.Sleep(300);
                }
                mutex.ReleaseMutex(); // Release the mutex
                empty.Release(); // Signal that an empty slot is available

                Thread.Sleep(consumerRate * 1000);
            }
        }

        private void fight2_Load(object sender, EventArgs e)
        {



        }

        private void userControl11_Load(object sender, EventArgs e)
        {

        }

        private void loading_warriors1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.textBox2.Text = (DateTime.Now - start).ToString(@"s\.fff");
            double percentage = (full.CurrentCount * 8);
            textBox1.Text = percentage.ToString("0.##") + "%";
            textBox3.Text = waiting.ToString();
            if (numberOfWaits == 0)
                textBox4.Text = "0";
            else
                textBox4.Text = (totalWaitingTime / numberOfWaits).ToString() + " ms";

        }
    }
}
