// See https://aka.ms/new-console-template for more information

using GestioneSpeseAdo.ConsoleApp;

Console.WriteLine("Benvenuti nel sistema di gestione delle spese.");

bool quit = false;
do
{
    Console.WriteLine($"============= Menu =============");
    Console.WriteLine();
    Console.WriteLine("Digita 1 per inserire una nuova spesa.\n");
    Console.WriteLine("Digita 2 per approvare una spesa esistente.\n");
    Console.WriteLine("Digita 3 per cancellare una spesa.\n");
    Console.WriteLine("Digita 4 per mostrare l'elenco delle spese approvate.\n");
    Console.WriteLine("Digita 5 per mostrare l'elenco delle spese di uno specifico utente.\n");
    Console.WriteLine("Digita 6 per mostrare il totale delle spese per categoria.\n");
    Console.WriteLine("Digita q per uscire.\n");


    // scelta utente
    Console.Write("> ");
    string scelta = Console.ReadLine();
    Console.WriteLine();

    switch (scelta)
    {
        case "1":
            DisconnectedMode.InsertSpesa();
            break;
        case "2":
            ConnectedMode.ApproveSpesa();
            break;
        case "3":
            ConnectedMode.DeleteSpesa();
            break;
        case "4":
            ConnectedMode.ShowSpeseApproved();
            break;
        case "5":
            ConnectedMode.ShowSpeseUser();
            break;
        case "6":
            ConnectedMode.TotSpeseByCategory();
            break;
        case "q":
            quit = true;
            break;
        default:
            Console.WriteLine("Hai digitato un tasto non suggerito.");
            break;
    }

} while (!quit);


