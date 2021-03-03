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

using System;
using Nethermind.Int256;

namespace Nethermind.Consensus
{
    public interface IPreparingBlockContextService
    {
        void SetContext(UInt256 baseFee, long blockNumber);
        public UInt256 BaseFee { get; }
        
        public long BlockNumber { get; }
    }

    public class PreparingBlockContextService : IPreparingBlockContextService
    {
        private PreparingBlockContext? _currentContext;
        
        public void SetContext(UInt256 baseFee, long blockNumber)
        {
            _currentContext = new PreparingBlockContext(baseFee, blockNumber);
        }

        public UInt256 BaseFee
        {
            get
            {
                if (_currentContext == null)
                {
                    throw new InvalidOperationException("Cannot use preparing block context, because it wasn't set");
                }
                
                return _currentContext.Value.BaseFee;
            }
        }

        public long BlockNumber         
        {
            get
            {
                if (_currentContext == null)
                {
                    throw new InvalidOperationException("Cannot use preparing block context, because it wasn't set");
                }
                
                return _currentContext.Value.BlockNumber;
            }
        }
    }
}
