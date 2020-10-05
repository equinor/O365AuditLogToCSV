using System;

namespace AuditLog2CSV {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Audit Log to CSV transformer");
            Console.WriteLine("");
            Console.WriteLine("Enter input filename/path: ");
            string inputFile = Console.ReadLine();

            Transformer transformMng = new Transformer();
            transformMng.TransformFileToCSV(inputFile);

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}
