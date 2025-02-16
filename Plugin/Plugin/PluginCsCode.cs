using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Plugin
{
    public class PluginCsCode : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //var context = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
            //var entity = (Entity)context.InputParameters["Target"];
            //var address1Line3 = (string)entity["address1_line3"];
            //entity["address1_line3"] = $"modified {address1Line3}";


            //COPY THE ACCOUNT TO NEW TABLE "xx_accountcopy"
            var context = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
            var orgFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
            var orgService = orgFactory.CreateOrganizationService(context.UserId);

            var tEntity = (Entity)context.InputParameters["Target"];

            var caName = tEntity["name"];
            var naName = $"{caName} (Copy)";


            var acName = "xx_accountcopy";
            var newAccount = new Entity(acName);
            newAccount["xx_name"] = naName;
            newAccount["xx_fax"] = "123-4567";

            //ONE WAY
            Guid accountID = orgService.Create(newAccount);


            //TWO WAY
            var request = new CreateRequest { Target = newAccount };
            var response = (CreateResponse)orgService.Execute(request);
            var aID = response.id;


            //UPDATE
            var exAccount = new Entity(acName, aID);
            var updateAccount = new Entity(acName);
            updateAccount.Id = exAccount.Id;
            updateAccount["fax"] = "765-4321";
            orgService.Update(updateAccount);

            //updateAccount.RowVersion = exAccount.RowVersion;
            //var requestU = new UpdateRequest { Target = updateAccount, ConcurrencyBehavior = ConcurrencyBehavior.IfRowVersionMatches };
            //orgService.Execute(requestU);


            //DELETE
            orgService.Delete(acName, aID);


            acName = "account";

            //RETRIEVE
            var retEntity = orgService.Retrieve(acName, aID, new ColumnSet("name", "fax"));
            Console.WriteLine($"{retEntity["name"]} {retEntity["fax"]}");


            //RETRIEVE using alternative key ("name")
            var retReq = new RetrieveRequest
            {
                ColumnSet = new ColumnSet("name", "fax"),
                Target = new EntityReference(acName, "name", "My Account 11")
            };

            var respRet = (RetrieveResponse)orgService.Execute(retReq);
            var entity = respRet.Entity;
            Console.WriteLine($"{retEntity["name"]} {retEntity["fax"]}");


            //RETRIEVE using Composite Key ("name")
            var keyS = new KeyAttributeCollection
            {
                { "name", "Account 11" },
                { "fax", "123-4567" }
            };

            var retCReq = new RetrieveRequest
            {
                ColumnSet = new ColumnSet("name", "fax"),
                Target = new EntityReference(acName, keyS)
            };

            var respCRet = (RetrieveResponse)orgService.Execute(retCReq);
            var cEntity = respCRet.Entity;
            Console.WriteLine($"{cEntity["name"]} {cEntity["fax"]}");


            //RETRIEVE Multiple of Items with Columns

            //CONDITION
            var vCond = new ConditionExpression();
            vCond.AttributeName = "name";
            vCond.Operator = ConditionOperator.BeginsWith;
            vCond.Values.Add("Account");

            //FILTER
            var vFilt = new FilterExpression();
            vFilt.Conditions.Add(vCond);

            //QueryExpression
            var vQuery = new QueryExpression();
            //vQuery.ColumnSet.AllColumns = true;
            vQuery.ColumnSet.AddColumns("name", "fax");
            vQuery.Criteria.Filters.Add(vFilt);

            var vResult = orgService.RetrieveMultiple(vQuery);
            foreach (var ent in vResult.Entities)
            {
                Console.WriteLine($"{ent["name"]} {ent["fax"]}");
            }


            //UPSERT - INSERT OR UPDATE
            var upsertAccount = new Entity("xx_accountcopy", "name", "Latest Insert");
            upsertAccount["xx_fax"] = caName;
            var upsertReq = new UpsertRequest { Target = upsertAccount };
            var upsertRes = (UpsertResponse)orgService.Execute(upsertReq);
            if (upsertRes.RecordCreated) Console.WriteLine($"upsert done");
            else Console.WriteLine($"upsert failed");




        }

        //USE OF API - TWO DIFFERENT PLUGIN - BELOW


        //public void Execute(IServiceProvider serviceProvider)
        //{
        //    var context = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        //    var requestParameter = context.InputParameters["RequestParameter"];
        //    var resParameter = $"{requestParameter} - {DateTime.UtcNow}";
        //    context.OutputParameters["ResponseParameter"] = resParameter;
        //}


        //public void Execute(IServiceProvider serviceProvider)
        //{
        //    var context = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        //    var orgFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
        //    var orgService = orgFactory.CreateOrganizationService(context.UserId);

        //    var req = new OrganizationRequest("api_Name") { ["RequestParamter"] = "hi there" };
        //    var res = orgService.Execute(req);
        //    Console.Writeline($"{res["ResponseParameter"]}");

        //}
    }
}
