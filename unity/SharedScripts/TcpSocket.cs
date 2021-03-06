﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class TcpSocketException : Exception
{
    public TcpSocketException(string message) : base(message)
    {
    }
}

public class TcpSocketReceiveResult
{
    public readonly int Size;
    public readonly SocketError Error;

    public TcpSocketReceiveResult(int size, SocketError error)
    {
        Size = size;
        Error = error;
    }
}

public class TcpSocket
{
    public Socket Socket { get; private set; }

    public TcpSocket(Socket socket)
    {
        Socket = socket;
        socket.Blocking = false;
    }

    public void BindAndListen(int port)
    {
        int BACKLOG = 5;
        Socket.Bind(new IPEndPoint(IPAddress.Any, port));
        Socket.Listen(BACKLOG);
    }

    public TcpSocket Accept()
    {
        try
        {
            return new TcpSocket(Socket.Accept());
        }
        catch (SocketException e)
        {
            if (e.SocketErrorCode == SocketError.WouldBlock)
            {
                return null;
            }

            throw new TcpSocketException($"Error from TcpSocket.Accept(): {e.SocketErrorCode}");
        }
    }

    public async Task<bool> ConnectAsync(IPAddress address, int port)
    {
        try
        {
            await Socket.ConnectAsync(address, port);
            return true;
        }
        catch (SocketException e)
        {
            if (e.SocketErrorCode == SocketError.ConnectionRefused)
                return false;

            throw new TcpSocketException($"Error from TcpSocket.ConnectAsync(): {e.SocketErrorCode}");
        }
    }

    public TcpSocketReceiveResult Receive(byte[] buffer, int offset, int length)
    {
        SocketError socketError;
        int size = Socket.Receive(buffer, offset, length, SocketFlags.None, out socketError);
        return new TcpSocketReceiveResult(size, socketError);
    }

    public int Send(byte[] buffer)
    {
        SocketError socketError;
        int size = Socket.Send(buffer, 0, buffer.Length, SocketFlags.None, out socketError);

        if (socketError != SocketError.Success)
            throw new TcpSocketException($"Error from TcpSocket.Send(): {socketError}");

        return size;
    }
}