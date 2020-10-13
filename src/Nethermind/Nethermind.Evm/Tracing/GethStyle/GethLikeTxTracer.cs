﻿//  Copyright (c) 2018 Demerzel Solutions Limited
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

using System;
using System.Collections.Generic;
using System.Threading;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Core.Extensions;
using Nethermind.Int256;
using Nethermind.State;

namespace Nethermind.Evm.Tracing.GethStyle
{
    public class GethLikeTxTracer : ITxTracer
    {
        private GethTxTraceEntry _traceEntry;
        private readonly GethLikeTxTrace _trace = new GethLikeTxTrace();

        public GethLikeTxTracer(GethTraceOptions options) 
        {
            IsTracingStack = options.DisableStack;
            IsTracingMemory = options.DisableMemory;
            IsTracingOpLevelStorage = options.DisableStorage;
        }
        
        public bool IsTracingReceipt => true;
        bool ITxTracer.IsTracingActions => false;
        public bool IsTracingOpLevelStorage { get; }
        public bool IsTracingMemory { get; }
        bool ITxTracer.IsTracingInstructions => true;
        public bool IsTracingRefunds => false;
        public bool IsTracingCode => false;
        public bool IsTracingStack { get; }
        bool ITxTracer.IsTracingState => false;
        
        public bool IsTracingBlockHash => false;
        
        public void MarkAsSuccess(Address recipient, long gasSpent, byte[] output, LogEntry[] logs, Keccak stateRoot = null)
        {
            _trace.ReturnValue = output;
            _trace.Gas = gasSpent;
        }

        public void MarkAsFailed(Address recipient, long gasSpent, byte[] output, string error, Keccak stateRoot = null)
        {
            _trace.Failed = true;
            _trace.ReturnValue = output ?? Array.Empty<byte>();
        }

        public void StartOperation(int depth, long gas, Instruction opcode, int pc)
        {
            var previousTraceEntry = _traceEntry;
            _traceEntry = new GethTxTraceEntry();
            _traceEntry.Pc = pc;
            _traceEntry.Operation = Enum.GetName(typeof(Instruction), opcode);
            _traceEntry.Gas = gas;
            _traceEntry.Depth = depth;
            _trace.Entries.Add(_traceEntry);
            
            if (_traceEntry.Depth > (previousTraceEntry?.Depth ?? 0))
            {
                _traceEntry.Storage = new Dictionary<string, string>();
                _trace.StoragesByDepth.Push(previousTraceEntry != null ? previousTraceEntry.Storage : new Dictionary<string, string>());
            }
            else if (_traceEntry.Depth < (previousTraceEntry?.Depth ?? 0))
            {
                if (previousTraceEntry == null)
                {
                    throw new InvalidOperationException("Unexpected missing previous trace when leaving a call.");
                }
                    
                _traceEntry.Storage = new Dictionary<string, string>(_trace.StoragesByDepth.Pop());
            }
            else
            {
                if (previousTraceEntry == null)
                {
                    throw new InvalidOperationException("Unexpected missing previous trace on continuation.");
                }
                    
                _traceEntry.Storage = new Dictionary<string, string>(previousTraceEntry.Storage);    
            }
        }

        public void ReportOperationError(EvmExceptionType error)
        {
            _traceEntry.Error = GetErrorDescription(error);
        }
        
        private string GetErrorDescription(EvmExceptionType evmExceptionType)
        {
            switch (evmExceptionType)
            {
                case EvmExceptionType.None:
                    return null;
                case EvmExceptionType.BadInstruction:
                    return "BadInstruction";
                case EvmExceptionType.StackOverflow:
                    return "StackOverflow";
                case EvmExceptionType.StackUnderflow:
                    return "StackUnderflow";
                case EvmExceptionType.OutOfGas:
                    return "OutOfGas";
                case EvmExceptionType.InvalidSubroutineEntry:
                    return "InvalidSubroutineEntry";
                case EvmExceptionType.InvalidSubroutineReturn:
                    return "InvalidSubroutineReturn";
                case EvmExceptionType.InvalidJumpDestination:
                    return "BadJumpDestination";
                case EvmExceptionType.AccessViolation:
                    return "AccessViolation";
                case EvmExceptionType.StaticCallViolation:
                    return "StaticCallViolation";
                default:
                    return "Error";
            }
        }

        public void ReportOperationRemainingGas(long gas)
        {
            _traceEntry.GasCost = _traceEntry.Gas - gas;
        }

        public void SetOperationMemorySize(ulong newSize)
        {
            _traceEntry.UpdateMemorySize(newSize);
        }

        public void ReportMemoryChange(long offset, in Span<byte> data)
        {
        }

        public void ReportStorageChange(in Span<byte> key, in Span<byte> value)
        {
        }

        public void SetOperationStorage(Address address, UInt256 storageIndex, byte[] newValue, byte[] currentValue)
        {
            byte[] bigEndian = new byte[32];
            storageIndex.ToBigEndian(bigEndian);
            _traceEntry.Storage[bigEndian.ToHexString(false)] = newValue.PadLeft(32).ToHexString(false);
        }

        public void ReportSelfDestruct(Address address, UInt256 balance, Address refundAddress)
        {
            throw new NotSupportedException();
        }

        public void ReportBalanceChange(Address address, UInt256? before, UInt256? after)
        {
            throw new NotSupportedException();
        }

        public void ReportCodeChange(Address address, byte[] before, byte[] after)
        {
            throw new NotSupportedException();
        }

        public void ReportNonceChange(Address address, UInt256? before, UInt256? after)
        {
            throw new NotSupportedException();
        }

        public void ReportAccountRead(Address address)
        {
        }

        public void ReportStorageChange(StorageCell storageCell, byte[] before, byte[] after)
        {
            throw new NotSupportedException();
        }

        public void ReportAction(long gas, UInt256 value, Address @from, Address to, byte[] input, ExecutionType callType, bool isPrecompileCall = false)
        {
            throw new NotSupportedException();
        }

        public void ReportActionEnd(long gas, byte[] output)
        {
            throw new NotSupportedException();
        }

        public void ReportActionError(EvmExceptionType exceptionType)
        {
            throw new NotSupportedException();
        }

        public void ReportActionEnd(long gas, Address deploymentAddress, byte[] deployedCode)
        {
            throw new NotSupportedException();
        }

        public void ReportBlockHash(Keccak blockHash)
        {
            throw new NotSupportedException();
        }

        public void ReportByteCode(byte[] byteCode)
        {
            throw new NotSupportedException();
        }

        public void ReportGasUpdateForVmTrace(long refund, long gasAvailable)
        {
        }

        public void ReportRefund(long refund)
        {
            throw new NotSupportedException();
        }

        public void ReportExtraGasPressure(long extraGasPressure)
        {
            throw new NotImplementedException();
        }

        public void SetOperationStack(List<string> stackTrace)
        {
            _traceEntry.Stack = stackTrace;
        }

        public void ReportStackPush(in Span<byte> stackItem)
        {
        }

        public void SetOperationMemory(List<string> memoryTrace)
        {
            _traceEntry.Memory = memoryTrace;
        }

        public GethLikeTxTrace BuildResult()
        {
            return _trace;
        }
    }
}
