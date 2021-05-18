﻿//  Copyright (c) 2021 Demerzel Solutions Limited
//  This file is part of the Nethermind library.
// 
//  The Nethermind library is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  The Nethermind library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
// 

using FluentAssertions;
using Nethermind.Core;
using Nethermind.Core.Extensions;
using Nethermind.Core.Specs;
using Nethermind.Core.Test.Builders;
using Nethermind.Int256;
using Nethermind.Logging;
using Nethermind.Serialization.Rlp;
using Nethermind.Specs.Forks;
using NSubstitute;
using NUnit.Framework;

namespace Nethermind.Evm.Test
{
    // Alex Beregszaszi, Paweł Bylica, Andrei Maiboroda, Alexey Akhunov, Christian Reitwiessner, Martin Swende, "EIP-3541: Reject new contracts starting with the 0xEF byte," Ethereum Improvement Proposals, no. 3541, March 2021. [Online serial]. Available: https://eips.ethereum.org/EIPS/eip-3541.
    public class Eip3541Tests : VirtualMachineTestsBase
    {
        const long LondonTestBlockNumber = 4_370_000;
        protected override ISpecProvider SpecProvider
        {
            get
            {
                ISpecProvider specProvider = Substitute.For<ISpecProvider>();
                specProvider.GetSpec(Arg.Is<long>(x => x >= LondonTestBlockNumber)).Returns(London.Instance);
                specProvider.GetSpec(Arg.Is<long>(x => x < LondonTestBlockNumber)).Returns(Berlin.Instance);
                return specProvider;
            }
        }
        
        // [TestCase(false, false)]
        // [TestCase(true, false)]
        // [TestCase(false, true)]
        // [TestCase(true, true)]
        // public void Wrong_contract_creation_should_return_invalid_code_after_3541(bool eip3541Enabled, bool create2)
        // {
        //     TestState.CreateAccount(TestItem.AddressC, 100.Ether());
        //     
        //     byte[] salt = {4, 5, 6};
        //     byte[] code = Prepare.EvmCode
        //         .FromCode("0xEF")
        //         .Done;
        //     byte[] createContract = create2 ?
        //             Prepare.EvmCode.Create2(code, salt, UInt256.Zero).Done
        //             : Prepare.EvmCode.Create(code, UInt256.Zero).Done;
        //     
        //     _processor = new TransactionProcessor(SpecProvider, TestState, Storage, Machine, LimboLogs.Instance);
        //     long blockNumber = eip3541Enabled ? LondonTestBlockNumber : LondonTestBlockNumber - 1;
        //     (Block block, Transaction transaction) = PrepareTx(blockNumber, 100000, createContract);
        //
        //     transaction.GasPrice = 20.GWei();
        //     transaction.To = null;
        //     transaction.Data = createContract;
        //     TestAllTracerWithOutput tracer = CreateTracer();
        //     _processor.Execute(transaction, block.Header, tracer);
        //
        //     if (eip3541Enabled)
        //     {
        //         tracer.Error.Should().Be("InvalidCode");
        //         tracer.StatusCode.Should().Be(0);
        //     }
        //     else
        //     {
        //         tracer.Error.Should().Be(null);
        //         tracer.StatusCode.Should().Be(1);
        //     }
        // }
        //
        //
        // [Test]
        // public void Wrong_contract_creation()
        // {
        //     var rawTx = Bytes.FromHexString(
        //         "0x02f85c82066a0b0110830186a080808b61efef6000526010601ff3c080a07c2ba5a05122aad439dcc1f51ff20800a2ccee2fd9bc42f7317e6d8dd426ecf5a05a7e2f82c02d0eec42266e8c6bb66bc0a94c8b583c2d388dae11253a57df3fbe");
        //     var tx = Rlp.Decode<Transaction>(rawTx, RlpBehaviors.AllowUnsigned | RlpBehaviors.SkipTypedWrapping);
        //     
        //     TestState.CreateAccount(TestItem.AddressC, 100.Ether());
        //     
        //     byte[] salt = {4, 5, 6};
        //     byte[] code = Prepare.EvmCode
        //         .FromCode("0xEF")
        //         .Done;
        //     byte[] createContract = false ?
        //         Prepare.EvmCode.Create2(code, salt, UInt256.Zero).Done
        //         : Prepare.EvmCode.Create(code, UInt256.Zero).Done;
        //     tx.SenderAddress = TestItem.AddressC;
        //     tx.Nonce = 0;
        //     _processor = new TransactionProcessor(SpecProvider, TestState, Storage, Machine, LimboLogs.Instance);
        //     long blockNumber = true ? LondonTestBlockNumber : LondonTestBlockNumber - 1;
        //     (Block block, Transaction transaction) = PrepareTx(blockNumber, 100000, createContract);
        //     
        //     TestAllTracerWithOutput tracer = CreateTracer();
        //     _processor.Execute(tx, block.Header, tracer);
        //
        //     if (true)
        //     {
        //         tracer.Error.Should().Be("InvalidCode");
        //         tracer.StatusCode.Should().Be(0);
        //     }
        //     else
        //     {
        //         tracer.Error.Should().Be(null);
        //         tracer.StatusCode.Should().Be(1);
        //     }
        // }
    }
}
