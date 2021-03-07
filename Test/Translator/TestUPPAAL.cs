using Plat._T;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class TestUPPAAL
    {
        public static void BuildMemoryModel()
        {
            // Model Type define
            UpType ByteVec = new UpType("ByteVec");
            UpType SUCI = new UpType("SUCI");

            // Global Declaration
            UpDeclaration globalDeclaration = new UpDeclaration();
            globalDeclaration.Statements.Add(new UpAssignment("a", "1"));
            globalDeclaration.Statements.Add(new UpPass());
            globalDeclaration.Statements.Add(new UpAssignment("b[1].c", "2"));

            // Model Declaration
            UpInstantiation modelDeclaration = new UpInstantiation();
            modelDeclaration.Statements.Add(new UpConcurrency("p1", "p2", "p3"));

            // Root Project
            UpProject upProject = new UpProject(globalDeclaration, modelDeclaration);

            // Process: UE
            UpDeclaration UEDeclaration = new UpDeclaration();
            UEDeclaration.Statements.Add(new UpNewVar(ByteVec, "__bytevec__encrypted"));
            UEDeclaration.Statements.Add(new UpNewVar(ByteVec, "bv"));
            UEDeclaration.Statements.Add(new UpNewVar(SUCI, "suci"));

            UpPG UEPG = new UpPG();
            UpLocation UE_init = new UpLocation("init", true);
            UEPG.Locations.Add(UE_init);
            UpLocation UE_Sended = new UpLocation("Sended");
            UEPG.Locations.Add(UE_Sended);
            UEPG.Transitions.Add(new UpTransition(UE_init, UE_Sended));

            UpProcess UEProc = new UpProcess("UE", UEDeclaration, UEPG);
            upProject.Processes.Add(UEProc);

            // Take a look
            Console.WriteLine(upProject);
        }
    }
}
