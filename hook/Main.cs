using EasyHook;
using SapphireActorCapture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hook
{
    public class Main : IEntryPoint
    {
        // just use a P-Invoke implementation to get native API access
        // from C# (this step is not necessary for C++.NET)
        [DllImport("Ws2_32.dll")]
        static extern int recv(
                    IntPtr socketHandle,
                    IntPtr buf,
                    int count,
                    int socketFlags
            );

        [DllImport("Ws2_32.dll")]
        static extern int send(
                    IntPtr socketHandle,
                    IntPtr buf,
                    int count,
                    int socketFlags
            );

        [UnmanagedFunctionPointer(CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            SetLastError = true)]        
        delegate int Drecv(
                    IntPtr socketHandle,
                    IntPtr buf,
                    int count,
                    int socketFlags
            );

        [UnmanagedFunctionPointer(CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            SetLastError = true)]
        delegate int Dsend(
                    IntPtr socketHandle,
                    IntPtr buf,
                    int count,
                    int socketFlags
            );

        public int recv_Hooked(
                    IntPtr socketHandle,
                    IntPtr buf,
                    int count,
                    int socketFlags
            )
        {
            int len = recv(socketHandle, buf, count, socketFlags);
            
            byte[] b = new byte[len];
            Marshal.Copy(buf, b, 0, len);

            try
            {
                remoteInterface.GetRecv(b);
            }
            catch (Exception e) { return len; }
            
            return len;
        }

        public int send_Hooked(
                    IntPtr socketHandle,
                    IntPtr buf,
                    int count,
                    int socketFlags
            )
        {
            int len = send(socketHandle, buf, count, socketFlags);

            byte[] b = new byte[len];
            Marshal.Copy(buf, b, 0, len);

            try
            {
                remoteInterface.GetSend(b);
            }
            catch (Exception e) { return len; }

            return len;
        }

        string channelName;
        LocalHook recvHook;
        LocalHook sendHook;
        RemoteMon remoteInterface;

        public Main(RemoteHooking.IContext InContext, String InChannelName)
        {
            // connect to host...
            remoteInterface =
              RemoteHooking.IpcConnectClient<RemoteMon>(InChannelName);
        }

        public void Run(RemoteHooking.IContext InContext, String InChannelName)
        {
            // install hook...
            try
            {
                recvHook = LocalHook.Create(
                    LocalHook.GetProcAddress("Ws2_32.dll", "recv"),
                    new Drecv(recv_Hooked),
                    this);

                recvHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                sendHook = LocalHook.Create(
                    LocalHook.GetProcAddress("Ws2_32.dll", "send"),
                    new Dsend(send_Hooked),
                    this);

                sendHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
            }
            catch (Exception ExtInfo)
            {
                remoteInterface.ExceptionHandler(ExtInfo);
                return;
            }

            remoteInterface.IsInstalled(RemoteHooking.GetCurrentProcessId());
            
            try
            {
                RemoteHooking.WakeUpProcess();
            }
            catch (Exception ex)
            {
                remoteInterface.ExceptionHandler(ex);
            }

            while (true)
            {
                Thread.Sleep(1000);
            }
        }        

    }
}
