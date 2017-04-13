﻿// autogenerated: do NOT edit manually / do NOT commit to source control
using Zinc.WebServices.RestClient;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zn.Sample
{
    /// <summary />
    public partial class OneClient : ServiceClient
    {
        /// <summary />
        public OneClient() : base( "Sample" )
        {
        }

        /// <summary>
        /// First method of service one.
        /// </summary>
        public OneSvc.MethodOneResponse MethodOne( OneSvc.MethodOneRequest request )
        {
            return Invoke<OneSvc.MethodOneRequest,OneSvc.MethodOneResponse>( "MethodOne", request );
        }

        /// <summary>
        /// First method of service one.
        /// </summary>
        public Task<OneSvc.MethodOneResponse> MethodOneAsync( OneSvc.MethodOneRequest request )
        {
            return MethodOneAsync( request, CancellationToken.None );
        }

        /// <summary>
        /// First method of service one.
        /// </summary>
        public async Task<OneSvc.MethodOneResponse> MethodOneAsync( OneSvc.MethodOneRequest request, CancellationToken cancellationToken )
        {
            return await InvokeAsync<OneSvc.MethodOneRequest, OneSvc.MethodOneResponse>( "MethodOne", request, cancellationToken ).ConfigureAwait( false );
        }

        /// <summary>
        /// Third method of service one.
        /// </summary>
        public OneSvc.MethodThreeResponse MethodThree( OneSvc.MethodThreeRequest request )
        {
            return Invoke<OneSvc.MethodThreeRequest,OneSvc.MethodThreeResponse>( "MethodThree", request );
        }

        /// <summary>
        /// Third method of service one.
        /// </summary>
        public Task<OneSvc.MethodThreeResponse> MethodThreeAsync( OneSvc.MethodThreeRequest request )
        {
            return MethodThreeAsync( request, CancellationToken.None );
        }

        /// <summary>
        /// Third method of service one.
        /// </summary>
        public async Task<OneSvc.MethodThreeResponse> MethodThreeAsync( OneSvc.MethodThreeRequest request, CancellationToken cancellationToken )
        {
            return await InvokeAsync<OneSvc.MethodThreeRequest, OneSvc.MethodThreeResponse>( "MethodThree", request, cancellationToken ).ConfigureAwait( false );
        }
    }


    /// <summary />
    public class TwoClient : ServiceClient
    {
        /// <summary />
        public TwoClient() : base( "Sample" )
        {
        }

        /// <summary>
        /// First method of service two.
        /// </summary>
        public TwoSvc.MethodTwoResponse MethodTwo( TwoSvc.MethodTwoRequest request )
        {
            return Invoke<TwoSvc.MethodTwoRequest,TwoSvc.MethodTwoResponse>( "MethodTwo", request );
        }

        /// <summary>
        /// First method of service two.
        /// </summary>
        public Task<TwoSvc.MethodTwoResponse> MethodTwoAsync( TwoSvc.MethodTwoRequest request )
        {
            return MethodTwoAsync( request, CancellationToken.None );
        }

        /// <summary>
        /// First method of service two.
        /// </summary>
        public async Task<TwoSvc.MethodTwoResponse> MethodTwoAsync( TwoSvc.MethodTwoRequest request, CancellationToken cancellationToken )
        {
            return await InvokeAsync<TwoSvc.MethodTwoRequest, TwoSvc.MethodTwoResponse>( "MethodTwo", request, cancellationToken ).ConfigureAwait( false );
        }
    }


    namespace OneSvc
    {
        /// <summary />
        public class MethodOneRequest
        {
            /// <summary />
            public int Value { get; set; }
        }

        /// <summary />
        public class MethodOneResponse
        {
            /// <summary />
            public int Value { get; set; }
        }

        /// <summary />
        public class MethodThreeRequest
        {
            /// <summary />
            public StringValidations String { get; set; }

            /// <summary />
            public SimpleClass SimpleClass { get; set; }
        }

        /// <summary />
        public class MethodThreeResponse
        {
            /// <summary />
            public NestedClass NestedClass { get; set; }

            /// <summary />
            public NestedArray[] NestedArray { get; set; }
        }
    }


    namespace TwoSvc
    {
        /// <summary />
        public class MethodTwoRequest
        {
            /// <summary>
            /// Input date
            /// </summary>
            public DateTime InDate { get; set; }

            /// <summary>
            /// Input date and time
            /// </summary>
            public DateTime InDateTime { get; set; }

            /// <summary>
            /// Input enumerate.
            /// </summary>
            public TestEnum InEnum { get; set; }
        }

        /// <summary />
        public class MethodTwoResponse
        {
            /// <summary>
            /// A random string value.
            /// </summary>
            public string RandomString { get; set; }

            /// <summary>
            /// A name from the Matrix.
            /// </summary>
            public string DataString { get; set; }

            /// <summary>
            /// A random integer value.
            /// </summary>
            public int RandomInteger { get; set; }

            /// <summary>
            /// A random date value.
            /// </summary>
            public DateTime RandomDate { get; set; }

            /// <summary>
            /// A random date and time value.
            /// </summary>
            public DateTime RandomDateTime { get; set; }

            /// <summary />
            public System.Guid ActivityId { get; set; }

            /// <summary />
            public string AccessToken { get; set; }

            /// <summary />
            public System.Guid ExecutionId { get; set; }

            /// <summary>
            /// Output enumerate.
            /// </summary>
            public TestEnum OutEnum { get; set; }
        }
    }


    /// <summary />
    public class NestedArray
    {
        /// <summary />
        public string Value { get; set; }

        /// <summary />
        public NestedClass[] Nested { get; set; }
    }


    /// <summary />
    public class NestedClass
    {
        /// <summary />
        public string Value { get; set; }

        /// <summary />
        public NestedClass Nested { get; set; }
    }


    /// <summary />
    public class SimpleClass
    {
        /// <summary />
        public decimal? Value1 { get; set; }

        /// <summary />
        public decimal? Value2 { get; set; }
    }


    /// <summary />
    public class StringValidations
    {
        /// <summary>
        /// Must be 3 digits long.
        /// </summary>
        public string Regex { get; set; }

        /// <summary>
        /// Must be between 5 and 20 characters long.
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// Must be at least 5 characters long.
        /// </summary>
        public string LengthMin { get; set; }

        /// <summary>
        /// Must be at most 20 characters long.
        /// </summary>
        public string LengthMax { get; set; }
    }


    /// <summary />
    public enum TestEnum
    {
        /// <summary />
        First,
        /// <summary />
        Second,
        /// <summary />
        Third,
    }
}