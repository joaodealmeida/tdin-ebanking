﻿using ExchangeService.BankA;
using ExchangeService.InterBank;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using System.Net.Mail;
using System.Net;
using System.Messaging;
using System.IO;

namespace ExchangeService
{
    public partial class ExchangeServiceApp : Form
    {
        List<Order> ordersToExecute;
        List<Order> todaysExecutedOrders;
        BankAOpsClient bankAProxy;
        InterBankOpsClient proxy;
        string pass = "tdin2016";
        string dir = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
        string serializationFile;

        public ExchangeServiceApp()
        {
            ordersToExecute = new List<Order>();
            todaysExecutedOrders = new List<Order>();
            bankAProxy = new BankAOpsClient();
            bankAProxy.Open();
            //ordersToExecute = bankAProxy.getUnexecutedOrders().ToList();
            serializationFile = Path.Combine(dir, "Order.bin");
            load();
            receiveMessages();
            InitializeComponent();

            updateUnexecutedOrders();
        }

        public List<string> receiveMessages()
        {
            List<string> tmp = new List<string>();

            if (MessageQueue.Exists(@".\Private$\supervisor"))
            {

                MessageQueue messageQueue = new MessageQueue(@".\Private$\supervisor");
                System.Messaging.Message[] messages = messageQueue.GetAllMessages();

                string rec = "";
                //string sqlcmd;
                foreach (System.Messaging.Message message in messages)
                {
                    //string line;
                    message.Formatter = new System.Messaging.XmlMessageFormatter(new String[] { });
                    StreamReader sr = new StreamReader(message.BodyStream);

                    while (sr.Peek() >= 0)
                    {
                        //Ordem o = new Ordem();
                        rec += sr.ReadLine();

                    }
                    Console.WriteLine(rec + "\n");
                    string[] words = rec.Split('+');

                    DateTime datetime = DateTime.ParseExact(words[5], "yyyy-MM-dd HH:mm:ss", null);
                    Order o = new Order();
                    o.Id = Int32.Parse(words[1]);
                    o.Company_id = Int32.Parse(words[2]);
                    o.Type = words[3];
                    o.Quantity = Int32.Parse(words[4]);
                    o.Creation_date = datetime;
                    o.Client_id = Int32.Parse(words[6]);
                    o.Value = Int32.Parse(words[7]);
                    o.State = "unexecuted";
                    ordersToExecute.Add(o);
                    rec = "";

                }
                messageQueue.Purge();
            }
            return tmp;
        }

        private void maskedTextBox2_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void button_Click(object sender, EventArgs e)
        {
            try
            {
                string selecionado = dataGridUnexecuted.SelectedCells[0].Value.ToString();
                int id = Int32.Parse(selecionado);

                Order tempOrder = new Order();
                tempOrder.Id = id;
                tempOrder.Client_id = Int32.Parse(dataGridUnexecuted.SelectedCells[1].Value.ToString());
                DateTime dt;
                DateTime.TryParse(dataGridUnexecuted.SelectedCells[2].Value.ToString(), out dt);
                tempOrder.Creation_date = dt;
                tempOrder.Quantity = Int32.Parse(dataGridUnexecuted.SelectedCells[3].Value.ToString());
                tempOrder.State = dataGridUnexecuted.SelectedCells[4].Value.ToString();
                tempOrder.Type = dataGridUnexecuted.SelectedCells[5].Value.ToString();
                tempOrder.Value = Int32.Parse(dataGridUnexecuted.SelectedCells[6].Value.ToString());
                tempOrder.Execution_date = DateTime.Now;
                todaysExecutedOrders.Add(tempOrder);

                bankAProxy.updateStock(id);
                ordersToExecute.RemoveAt(dataGridUnexecuted.SelectedCells[0].RowIndex);
                updateUnexecutedOrders();
                updateExecutedOrders();
                Cliente client = bankAProxy.getClient(tempOrder.Client_id);
                sendEmail(client, tempOrder.Quantity, tempOrder.Value, tempOrder.Type);
                save();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception: " + exc.Message);
                MessageBox.Show("Nenhum Item selecionado", "Alerta");
            }
        }

        public void updateUnexecutedOrders()
        {
            try
            {
                //ordersToExecute = bankAProxy.getUnexecutedOrders().ToList();

                dataGridUnexecuted.Rows.Clear();
                foreach (Order order in ordersToExecute)
                {
                    Console.Write(order.Id);
                    string[] temp = { order.Id.ToString(), order.Client_id.ToString(), order.Creation_date.ToString(), order.Quantity.ToString(), order.State.ToString(), order.Type, order.Value.ToString()};
                    dataGridUnexecuted.Rows.Add(temp);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public void updateExecutedOrders()
        {
            try
            {
                dataGridExecuted.Rows.Clear();
                foreach (Order order in todaysExecutedOrders)
                {
                    Console.Write(order.Id);
                    string[] temp = { order.Id.ToString(), order.Client_id.ToString(), order.Creation_date.ToString(), order.Quantity.ToString(), "executed", order.Type, order.Value.ToString(), order.Execution_date.ToString() };
                    dataGridExecuted.Rows.Add(temp);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public void sendEmail(Cliente client, int quantity, double value, string type)
        {
            try
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.live.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("luismiguel667@hotmail.com", pass)
                };
                using (var message = new MailMessage("luismiguel667@hotmail.com", client.Email)
                {
                    Subject = "Your Order has been executed",
                    Body = "Dear "+ client.Name + ", \n Congratulations! Your order has been executed. Your " + type + " of " + quantity + " shares, for a total of " + value + "€. \n"
                })
                {
                    smtp.Send(message);
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            
            List<string> msg = receiveMessages();
            foreach (string s in msg)
            {
                Console.WriteLine("Message");
            }
            updateUnexecutedOrders();
            save();
        }

        private void save()
        {
            try
            {
                using (Stream stream = File.Open(serializationFile, FileMode.Create))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    bformatter.Serialize(stream, ordersToExecute);
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void load()
        {
            try
            {
                using (Stream stream = File.Open(serializationFile, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    ordersToExecute = (List<Order>)bformatter.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
