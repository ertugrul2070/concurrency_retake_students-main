using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace Concurrency_retake
{
    public class Cook
    {
        // you can add more code below this line


        internal Thread cookThread;


        // you can add more code above this line
        // Please do not alter the variables below
        private int cookId;
        private LinkedList<Order> orderLocation;
        private LinkedList<Portion> pickupPoint;
        private LinkedList<Portion> cookArms = new();
        public static LinkedList<Order> workingsurface = new();

        public Cook(int cookId, LinkedList<Order> orderLocation, LinkedList<Portion> pickupPoint)
        //you can alter the code and the parameters passed in here
        {
            this.orderLocation = orderLocation;
            this.pickupPoint = pickupPoint;
            this.cookId = cookId;
        }

        public void Start() //you can alter the code in here if needed
        {
            cookThread = new Thread(Life);
            cookThread.Start();

        }
        public void Life() //you should only add protections in this method
        {
            Thread.Sleep(new Random().Next(100, 300));

            Order? tempOrder;

            Console.WriteLine($"-Cook {cookId} is about to pick up an order.");
            lock (Program.orderLock)
            {
                while (orderLocation.Count == 0)
                {
                    Monitor.Wait(Program.orderLock);
                }
            tempOrder = orderLocation.First();

            orderLocation.RemoveFirst();
            }
            // Simulate cooking time
            Thread.Sleep(new Random().Next(100, 500));

            var toPrint = "";

            var done = false;

            tempOrder.StartWorking();

            // only up to "n_portions" orders can be on the working surface at any time
            // no more than "n_portions" cooks can be working at the same time
            // after n_portions cooks are done, another n_portions cooks can start working
            // they take turns in batches of n_portions size

            // Simulate cooking time
            Thread.Sleep(new Random().Next(100, 500));

            lock (Program.surfaceLock)
            {
                // if it is the last contribution
                if (workingsurface.Count == Program.n_portions - 1)
                {
                    // Console.WriteLine($"+Cook ............there are {workingsurface.Count} portions on the working surface");
                    // add the order to the working surface

                    tempOrder.FinishPortion();

                    workingsurface.AddFirst(tempOrder);

                    // remove the orders from the working surface and put them on the arm
                    foreach (var order in workingsurface)
                    {
                        cookArms.AddFirst(order.FinishWorking(Order.OrderPrepared.ToString()));
                    }

                    tempOrder = workingsurface.First();

                    workingsurface.RemoveFirst();

                    workingsurface.Clear();

                    toPrint = $"-Cook {cookId} is finishing order {tempOrder.ToString()} and will deliver {cookArms.Count} portions";
                }
                else
                {
                    // if it is the first n ontributions
                    // add the order to the working surface

                    tempOrder.FinishPortion();

                    workingsurface.AddFirst(tempOrder);

                    //toPrint = $"+Cook {cookId} is cooking order {tempOrder.ToString()}";
                    done = true;
                }
            }

            if (done)
            {

                Console.WriteLine($"-Cook {cookId} is going to rest forever.");

                return;
            }

            Console.WriteLine(toPrint);

            //add a part of a portion to the order
            // it is finally ready

            Console.WriteLine($"-Cook {cookId} is done cooking.");

            // add the portion to the pickup point tray
            // feel free to alter the "ID" to something meaningful to you for debugging purposes
            lock (Program.pickupLock)
            {
                foreach (var tempPortion in cookArms)
                {
                    pickupPoint.AddFirst(tempPortion);
                }
                Monitor.PulseAll(Program.pickupLock);
            }

            Console.WriteLine($"-Cook {cookId} has delivered {cookArms.Count} orders.");

            // clear the arms
            cookArms.Clear();

            Console.WriteLine($"-Cook {cookId} is going to rest forever.");
        }
    }
}
