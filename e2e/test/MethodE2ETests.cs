﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.E2ETests
{
    [TestClass]
    [TestCategory("IoTHub-E2E")]
    public class MethodE2ETests : IDisposable
    {
        private readonly string DevicePrefix = $"E2E_{nameof(MethodE2ETests)}_";
        private readonly int MuxDevicesCount = 10;
        private readonly int MuxWithoutPoolingPoolSize = 1;
        private readonly int MuxWithPoolingPoolSize = 5;
        private const string DeviceResponseJson = "{\"name\":\"e2e_test\"}";
        private const string ServiceRequestJson = "{\"a\":123}";
        private const string MethodName = "MethodE2ETest";
        private static TestLogging _log = TestLogging.GetInstance();

        private readonly ConsoleEventListener _listener;

        public MethodE2ETests()
        {
            _listener = TestConfig.StartEventListener();
        }

        [TestMethod]
        public async Task Method_DeviceReceivesMethodAndResponse_Mqtt()
        {
            await SendMethodAndRespond(Client.TransportType.Mqtt_Tcp_Only, SetDeviceReceiveMethod).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Method_DeviceReceivesMethodAndResponse_MqttWs()
        {
            await SendMethodAndRespond(Client.TransportType.Mqtt_WebSocket_Only, SetDeviceReceiveMethod).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Method_DeviceReceivesMethodAndResponseWithObseletedSetMethodHandler_Mqtt()
        {
            await SendMethodAndRespond(Client.TransportType.Mqtt_Tcp_Only, SetDeviceReceiveMethodObsoleteHandler).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Method_DeviceReceivesMethodAndResponseWithObseletedSetMethodHandler_MqttWs()
        {
            await SendMethodAndRespond(Client.TransportType.Mqtt_WebSocket_Only, SetDeviceReceiveMethodObsoleteHandler).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Method_DeviceReceivesMethodAndResponseWithDefaultMethodHandler_Mqtt()
        {
            await SendMethodAndRespond(Client.TransportType.Mqtt_Tcp_Only, SetDeviceReceiveMethodDefaultHandler).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Method_DeviceReceivesMethodAndResponseWithDefaultMethodHandler_MqttWs()
        {
            await SendMethodAndRespond(Client.TransportType.Mqtt_WebSocket_Only, SetDeviceReceiveMethodDefaultHandler).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Method_DeviceReceivesMethodAndResponse_Amqp()
        {
            await SendMethodAndRespond(Client.TransportType.Amqp_Tcp_Only, SetDeviceReceiveMethod).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Method_DeviceReceivesMethodAndResponse_AmqpWs()
        {
            await SendMethodAndRespond(Client.TransportType.Amqp_WebSocket_Only, SetDeviceReceiveMethod).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Method_DeviceReceivesMethodAndResponseWithObseletedSetMethodHandler_Amqp()
        {
            await SendMethodAndRespond(Client.TransportType.Amqp_Tcp_Only, SetDeviceReceiveMethodObsoleteHandler).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Method_DeviceReceivesMethodAndResponseWithObseletedSetMethodHandler_AmqpWs()
        {
            await SendMethodAndRespond(Client.TransportType.Amqp_WebSocket_Only, SetDeviceReceiveMethodObsoleteHandler).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Method_DeviceReceivesMethodAndResponseWithDefaultMethodHandler_Amqp()
        {
            await SendMethodAndRespond(Client.TransportType.Amqp_Tcp_Only, SetDeviceReceiveMethodDefaultHandler).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task Method_DeviceReceivesMethodAndResponseWithDefaultMethodHandler_AmqpWs()
        {
            await SendMethodAndRespond(Client.TransportType.Amqp_WebSocket_Only, SetDeviceReceiveMethodDefaultHandler).ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("ConnectionPoolingE2ETests")]
        public async Task Method_DeviceSak_DeviceReceivesMethodAndResponse_MuxWithoutPooling_Amqp()
        {
            await SendMethodAndRespondMuxedOverAmqp(
                TestDeviceType.Sasl,
                ConnectionStringLevel.Device,
                Client.TransportType.Amqp_Tcp_Only,
                MuxWithoutPoolingPoolSize,
                MuxDevicesCount,
                SetDeviceReceiveMethod
                ).ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("ConnectionPoolingE2ETests")]
        public async Task Method_DeviceSak_DeviceReceivesMethodAndResponse_MuxWithoutPooling_AmqpWs()
        {
            await SendMethodAndRespondMuxedOverAmqp(
                TestDeviceType.Sasl,
                ConnectionStringLevel.Device,
                Client.TransportType.Amqp_WebSocket_Only,
                MuxWithoutPoolingPoolSize,
                MuxDevicesCount,
                SetDeviceReceiveMethod
                ).ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("ConnectionPoolingE2ETests")]
        public async Task Method_IoTHubSak_DeviceReceivesMethodAndResponse_MuxWithoutPooling_Amqp()
        {
            await SendMethodAndRespondMuxedOverAmqp(
                TestDeviceType.Sasl,
                ConnectionStringLevel.IoTHub,
                Client.TransportType.Amqp_Tcp_Only,
                MuxWithoutPoolingPoolSize,
                MuxDevicesCount,
                SetDeviceReceiveMethod
                ).ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("ConnectionPoolingE2ETests")]
        public async Task Method_IoTHubSak_DeviceReceivesMethodAndResponse_MuxWithoutPooling_AmqpWs()
        {
            await SendMethodAndRespondMuxedOverAmqp(
                TestDeviceType.Sasl,
                ConnectionStringLevel.IoTHub,
                Client.TransportType.Amqp_WebSocket_Only,
                MuxWithoutPoolingPoolSize,
                MuxDevicesCount,
                SetDeviceReceiveMethod
                ).ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("ConnectionPoolingE2ETests")]
        public async Task Method_DeviceSak_DeviceReceivesMethodAndResponse_MuxWithPooling_Amqp()
        {
            await SendMethodAndRespondMuxedOverAmqp(
                TestDeviceType.Sasl,
                ConnectionStringLevel.Device,
                Client.TransportType.Amqp_Tcp_Only,
                MuxWithPoolingPoolSize, MuxDevicesCount,
                SetDeviceReceiveMethod
                ).ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("ConnectionPoolingE2ETests")]
        public async Task Method_DeviceSak_DeviceReceivesMethodAndResponse_MuxWithPooling_AmqpWs()
        {
            await SendMethodAndRespondMuxedOverAmqp(
                TestDeviceType.Sasl,
                ConnectionStringLevel.Device,
                Client.TransportType.Amqp_WebSocket_Only,
                MuxWithPoolingPoolSize,
                MuxDevicesCount,
                SetDeviceReceiveMethod
                ).ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("ConnectionPoolingE2ETests")]
        public async Task Method_IoTHubSak_DeviceReceivesMethodAndResponse_MuxWithPooling_Amqp()
        {
            await SendMethodAndRespondMuxedOverAmqp(
                TestDeviceType.Sasl,
                ConnectionStringLevel.IoTHub,
                Client.TransportType.Amqp_Tcp_Only,
                MuxWithPoolingPoolSize,
                MuxDevicesCount,
                SetDeviceReceiveMethod
                ).ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("ConnectionPoolingE2ETests")]
        public async Task Method_IoTHubSak_DeviceReceivesMethodAndResponse_MuxWithPooling_AmqpWs()
        {
            await SendMethodAndRespondMuxedOverAmqp(
                TestDeviceType.Sasl,
                ConnectionStringLevel.IoTHub,
                Client.TransportType.Amqp_WebSocket_Only,
                MuxWithPoolingPoolSize,
                MuxDevicesCount,
                SetDeviceReceiveMethod
                ).ConfigureAwait(false);
        }

        private async Task ServiceSendMethodAndVerifyResponse(string deviceName, string methodName, string respJson, string reqJson)
        {
            using (ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(Configuration.IoTHub.ConnectionString))
            {
                _log.WriteLine($"{nameof(ServiceSendMethodAndVerifyResponse)}: Invoke method {methodName}.");
                CloudToDeviceMethodResult response =
                    await serviceClient.InvokeDeviceMethodAsync(
                        deviceName,
                        new CloudToDeviceMethod(methodName, TimeSpan.FromMinutes(5)).SetPayloadJson(reqJson)).ConfigureAwait(false);

                _log.WriteLine($"{nameof(ServiceSendMethodAndVerifyResponse)}: Method status: {response.Status}.");
                Assert.AreEqual(200, response.Status);
                Assert.AreEqual(respJson, response.GetPayloadAsJson());

                await serviceClient.CloseAsync().ConfigureAwait(false);
            }
        }

        private async Task<Task> SetDeviceReceiveMethod(DeviceClient deviceClient)
        {
            var methodCallReceived = new TaskCompletionSource<bool>();

            await deviceClient.SetMethodHandlerAsync(MethodName,
                (request, context) =>
                {
                    _log.WriteLine($"{nameof(SetDeviceReceiveMethod)}: DeviceClient method: {request.Name} {request.ResponseTimeout}.");

                    try
                    {
                        Assert.AreEqual(MethodName, request.Name);
                        Assert.AreEqual(ServiceRequestJson, request.DataAsJson);

                        methodCallReceived.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        methodCallReceived.SetException(ex);
                    }
                    
                    return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(DeviceResponseJson), 200));
                },
                null).ConfigureAwait(false);
            
            // Return the task that tells us we have received the callback.
            return methodCallReceived.Task;
        }

        private async Task<Task> SetDeviceReceiveMethodDefaultHandler(DeviceClient deviceClient)
        {
            var methodCallReceived = new TaskCompletionSource<bool>();

            await deviceClient.SetMethodDefaultHandlerAsync(
                (request, context) =>
                {
                    _log.WriteLine($"{nameof(SetDeviceReceiveMethodDefaultHandler)}: DeviceClient method: {request.Name} {request.ResponseTimeout}.");

                    try
                    {
                        Assert.AreEqual(MethodName, request.Name);
                        Assert.AreEqual(ServiceRequestJson, request.DataAsJson);

                        methodCallReceived.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        methodCallReceived.SetException(ex);
                    }

                    return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(DeviceResponseJson), 200));
                },
                null).ConfigureAwait(false);

            return methodCallReceived.Task;
        }

        private Task<Task> SetDeviceReceiveMethodObsoleteHandler(DeviceClient deviceClient)
        {
            var methodCallReceived = new TaskCompletionSource<bool>();

#pragma warning disable CS0618
            deviceClient.SetMethodHandler(MethodName, (request, context) =>
            {
                _log.WriteLine($"{nameof(SetDeviceReceiveMethodObsoleteHandler)}: DeviceClient method: {request.Name} {request.ResponseTimeout}.");

                try
                {
                    Assert.AreEqual(MethodName, request.Name);
                    Assert.AreEqual(ServiceRequestJson, request.DataAsJson);

                    methodCallReceived.SetResult(true);
                }
                catch (Exception ex)
                {
                    methodCallReceived.SetException(ex);
                }

                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(DeviceResponseJson), 200));
            }, null);
#pragma warning restore CS0618

            return Task.FromResult<Task>(methodCallReceived.Task);
        }

        private async Task SendMethodAndRespond(Client.TransportType transport, Func<DeviceClient, Task<Task>> setDeviceReceiveMethod)
        {
            TestDevice testDevice = await TestDevice.GetTestDeviceAsync(DevicePrefix).ConfigureAwait(false);

            using (DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(testDevice.ConnectionString, transport))
            {
                Task methodReceivedTask = await setDeviceReceiveMethod(deviceClient).ConfigureAwait(false);

                await Task.WhenAll(
                    ServiceSendMethodAndVerifyResponse(testDevice.Id, MethodName, DeviceResponseJson, ServiceRequestJson),
                    methodReceivedTask).ConfigureAwait(false);

                await deviceClient.CloseAsync().ConfigureAwait(false);
            }
        }

        private async Task SendMethodAndRespondMuxedOverAmqp(
            TestDeviceType type,
            ConnectionStringLevel connectionStringLevel,
            Client.TransportType transport,
            int poolSize,
            int devicesCount,
            Func<DeviceClient, Task<Task>> setDeviceReceiveMethod
            )
        {
            var transportSettings = new ITransportSettings[]
            {
                new AmqpTransportSettings(transport)
                {
                    AmqpConnectionPoolSettings = new AmqpConnectionPoolSettings()
                    {
                        MaxPoolSize = unchecked((uint)poolSize),
                        Pooling = true
                    }
                }
            };

            DeviceClient[] deviceClients = new DeviceClient[devicesCount];

            try
            {
                for (int i = 0; i < devicesCount; i++)
                {
                    TestDevice testDevice = await TestDevice.GetTestDeviceAsync($"{DevicePrefix}_{i}_", type).ConfigureAwait(false);
                    DeviceClient deviceClient = testDevice.CreateDeviceClient(transportSettings, connectionStringLevel);
                    deviceClients[i] = deviceClient;
                    Task methodReceivedTask = await setDeviceReceiveMethod(deviceClient).ConfigureAwait(false);

                    await Task.WhenAll(
                        ServiceSendMethodAndVerifyResponse(testDevice.Id, MethodName, DeviceResponseJson, ServiceRequestJson),
                        methodReceivedTask).ConfigureAwait(false);
                }
            }
            finally
            {
                // Close and dispose all of the device client instances here
                foreach (DeviceClient deviceClient in deviceClients)
                {
                    await deviceClient.CloseAsync().ConfigureAwait(false);
                    _log.WriteLine($"{nameof(MethodE2ETests)}: Disposing deviceClient {TestLogging.GetHashCode(deviceClient)}");
                    deviceClient.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
