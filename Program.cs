using System;
using System.Collections.Generic;
using System.Threading;

namespace Concurrency_retake // Removing this comment will result in NVL
{
    class Program
    {//the removal or alteration of any code that is not explicitly allowed will result in a fail.

        //feel free to alter the value of the variables below, but do not alter the code itself.
        // remember to put them to the original value before submitting the code.
        public static int n_portions = 3; // Number of portions per tray, maximum amount of customers per tray
        private static int n_clients = 1998;
        private static int n_cooks = 1998; // Number of cooks will always be the same as the number of clients
        // you can add code below this line

        // Add these variables
        public static object orderLock = new object();
        public static object pickupLock = new object();
        public static object surfaceLock = new object();

        private static Thread[] clientThreads = new Thread[n_clients];
        private static Thread[] cookThreads = new Thread[n_cooks];

        // do not alter the code below
        private static Client[] clients = new Client[n_clients]; // Array of orders
        private static Cook[] cooks = new Cook[n_cooks]; // Array of cooks
        private static LinkedList<Portion> pickupPoint = new();
        private static LinkedList<Order> orderLocation = new();
        static void Main(string[] args)
        {// DO NOT alter the code below
            n_cooks = n_clients;

            if (n_clients % n_portions != 0)
            {
                throw new Exception("n_customers must be a multiple of n_portions");
            }
            //init environment variables here if needed

            //init Cooks and clients
            InitPeople();
            //activate Cooks
            ActivateCooks();
            //activate clients
            ActivateClients();
            // do not alter the code above
            // you can add more code below




            // you can add more code above this line
            // Print statistics DO NOT alter this code.
            Console.WriteLine("------Final stats------");
            Console.WriteLine($"Orders remainings: {orderLocation.Count}");
            Console.WriteLine($"Items at pickup tray: {pickupPoint.Count}");
            Console.WriteLine($"Orders on working surface: {Cook.workingsurface.Count}");
            Console.WriteLine($"Orders prepared: {Order.OrderPrepared}");
            Console.WriteLine($"Portions consumed: {Portion.PortionCounter}");
        }

        private static void ActivateClients()// activate clients
        {
            for (int i = 0; i < n_clients; i++)
            {
                clients[i].Start();
                clientThreads[i] = clients[i].clientThread;
            }

            for (int i = 0; i < n_clients; i++)
            {
                clientThreads[i].Join();
            }

            for (int i = 0; i < n_cooks; i++)
            {
                cookThreads[i].Join();
            }
        }

        private static void ActivateCooks() //activate cooks
        {
            for (int i = 0; i < n_cooks; i++)
            {
                cooks[i].Start();
                cookThreads[i] = cooks[i].cookThread;
            }
        }

        private static void InitPeople() //init clients and cooks
        {
            for (int i = 0; i < n_clients; i++)
            {
                clients[i] = new Client(i, orderLocation, pickupPoint);
                cooks[i] = new Cook(i, orderLocation, pickupPoint);
            }
        }
    }
    public class Order
    { //DO NOT ALTER THIS CLASS
        public static int OrderPrepared{ get; private set; } = 0;
        public OrderState State { get; private set; }

        public PortionState PortionStateval { get; private set; }
        public Order()
        {
            PortionStateval = PortionState.not_yet_prepared;
            State = OrderState.Ordered;
        }

        public void StartWorking()
        {
            State = OrderState.Working;
        }

        public Portion FinishWorking(string portionId)
        {
            State = OrderState.Ready;
            OrderPrepared++;
            return new Portion(portionId);
        }

        public void FinishPortion()
        {
                PortionStateval = PortionState.done;
        }
    }

    public class Portion //DO NOT ALTER THIS CLASS
    {
        private string _id;
        public static int PortionCounter { get; private set; } = 0;
        public Portion(string id)
        {
            //constructor
            _id = id;
            PortionCounter++;
        }
        public override string ToString()
        {
            return _id;
        }

    }
    public enum OrderState
    {
        Ordered,
        Working,
        Ready
    }

    public enum PortionState
    {
        not_yet_prepared,
        done
    }
}

