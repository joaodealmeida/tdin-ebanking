﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Server.BankB {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BankB.IBankBOps", SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface IBankBOps {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBankBOps/Deposit", ReplyAction="http://tempuri.org/IBankBOps/DepositResponse")]
        [System.ServiceModel.TransactionFlowAttribute(System.ServiceModel.TransactionFlowOption.Allowed)]
        void Deposit(int acct, double amount);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBankBOps/Withdraw", ReplyAction="http://tempuri.org/IBankBOps/WithdrawResponse")]
        [System.ServiceModel.TransactionFlowAttribute(System.ServiceModel.TransactionFlowOption.Allowed)]
        void Withdraw(int acct, double amount);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBankBOps/GetBalance", ReplyAction="http://tempuri.org/IBankBOps/GetBalanceResponse")]
        double GetBalance(int acct);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBankBOpsChannel : Server.BankB.IBankBOps, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BankBOpsClient : System.ServiceModel.ClientBase<Server.BankB.IBankBOps>, Server.BankB.IBankBOps {
        
        public BankBOpsClient() {
        }
        
        public BankBOpsClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public BankBOpsClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BankBOpsClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BankBOpsClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void Deposit(int acct, double amount) {
            base.Channel.Deposit(acct, amount);
        }
        
        public void Withdraw(int acct, double amount) {
            base.Channel.Withdraw(acct, amount);
        }
        
        public double GetBalance(int acct) {
            return base.Channel.GetBalance(acct);
        }
    }
}
