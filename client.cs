using System;
using System.Threading;

namespace Concurrency_retake
{
    public class Client
    {
        // you can add more code below this line

        internal Thread clientThread;

        // you can add more code above this line
        // Please do not alter the variables below
        private int clientId;

        private LinkedList<Order> orderLocation;
        private LinkedList<Portion> pickupPoint;

        public Client(int clientId, LinkedList<Order> orderLocation, LinkedList<Portion> pickupPoint)
        //you can alter the code and the parametersin here
        {
            this.orderLocation = orderLocation;
            this.pickupPoint = pickupPoint;
            this.clientId = clientId;
        }

        public void Start() //you can alter the code in here if needed
        {
            clientThread = new Thread(Life);
            clientThread.Start();
        }

        public void Life() //you should only add protections in this method
        {
            Console.WriteLine($"+Client {clientId} is about to place an order.");
            // order placement
            Order order = new();
            lock (Program.orderLock)
            {
                orderLocation.AddFirst(order);
                Monitor.Pulse(Program.orderLock);
            }
            Console.WriteLine($"+Client {clientId} has placed an order.");

            // waiting for the preparation of the order
            Thread.Sleep(new Random().Next(100, 500));


            Portion? tmpmyfood;

            // order pickup from the tray
            Console.WriteLine($"+Client {clientId} is about topick up the order.");
            lock (Program.pickupLock)
            {
                while (pickupPoint.Count == 0)
                {
                    Monitor.Wait(Program.pickupLock);
                }
                tmpmyfood = pickupPoint.First();
                pickupPoint.RemoveFirst();
            }
            // the order is picked up
            // the client can now leave.

            Console.WriteLine($"+Client {clientId} has finished {tmpmyfood.ToString()}.");

        }
    }
}
