﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection.Runtime.General;
using System.Reflection.Runtime.General.NativeFormat;
using System.Reflection.Runtime.CustomAttributes;

using Internal.Reflection.Core;
using Internal.Reflection.Core.Execution;

using Internal.Metadata.NativeFormat;

namespace System.Reflection.Runtime.ParameterInfos.NativeFormat
{
    //
    // This implements ParameterInfo objects owned by MethodBase objects that have an associated Parameter metadata entity.
    //
    internal sealed partial class NativeFormatMethodParameterInfo : RuntimeFatMethodParameterInfo
    {
        private NativeFormatMethodParameterInfo(MethodBase member, MethodHandle methodHandle, int position, ParameterHandle parameterHandle, QSignatureTypeHandle qualifiedParameterTypeHandle, TypeContext typeContext)
            : base(member, position, qualifiedParameterTypeHandle, typeContext)
        {
            _methodHandle = methodHandle;
            _parameterHandle = parameterHandle;
            _parameter = parameterHandle.GetParameter(Reader);
        }

        private MetadataReader Reader
        {
            get
            {
                Debug.Assert(QualifiedParameterTypeHandle.Reader is MetadataReader);
                return (MetadataReader)QualifiedParameterTypeHandle.Reader;
            }
        }

        public sealed override ParameterAttributes Attributes
        {
            get
            {
                return _parameter.Flags;
            }
        }

        public sealed override IEnumerable<CustomAttributeData> CustomAttributes
        {
            get
            {
                IEnumerable<CustomAttributeData> customAttributes = RuntimeCustomAttributeData.GetCustomAttributes(this.Reader, _parameter.CustomAttributes);
                foreach (CustomAttributeData cad in customAttributes)
                    yield return cad;
                MethodHandle declaringMethodHandle = _methodHandle;
                foreach (CustomAttributeData cad in ReflectionCoreExecution.ExecutionEnvironment.GetPseudoCustomAttributes(this.Reader, _parameterHandle, declaringMethodHandle))
                    yield return cad;
            }
        }

        public sealed override String Name
        {
            get
            {
                return _parameter.Name.GetStringOrNull(this.Reader);
            }
        }

        public sealed override int MetadataToken
        {
            get
            {
                throw new InvalidOperationException(SR.NoMetadataTokenAvailable);
            }
        }

        protected sealed override bool GetDefaultValueIfAvailable(bool raw, out object defaultValue)
        {
            return DefaultValueParser.GetDefaultValueIfAny(Reader, _parameter.DefaultValue, ParameterType, CustomAttributes, raw, out defaultValue);
        }

        private readonly MethodHandle _methodHandle;
        private readonly ParameterHandle _parameterHandle;
        private readonly Parameter _parameter;
    }
}
