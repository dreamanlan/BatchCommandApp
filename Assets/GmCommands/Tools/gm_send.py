#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
GM Command Sender for HarmonyOS devices.

Usage:
    # 1. Setup port forwarding first:
    #    hdc forward tcp:39527 tcp:39527
    #
    # 2. Send a single command:
    #    python gm_send.py "your_gm_command_here"
    #
    # 3. Send multiple commands from a file:
    #    python gm_send.py -f commands.txt
    #
    # 4. Interactive mode:
    #    python gm_send.py -i
    #
    # 5. Auto setup port forwarding and send:
    #    python gm_send.py --setup "your_gm_command_here"
"""

import argparse
import os
import socket
import subprocess
import sys


DEFAULT_HOST = "127.0.0.1"
DEFAULT_PORT = 39527
TIMEOUT = 5


def send_command(host, port, cmd):
    """Send a GM command to the game via TCP socket."""
    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        s.settimeout(TIMEOUT)
        s.connect((host, port))
        s.sendall(cmd.encode("utf-8"))
        s.shutdown(socket.SHUT_WR)
        s.close()
        print(f"[OK] Sent: {cmd}")
        return True
    except ConnectionRefusedError:
        print(f"[ERROR] Connection refused. Is the game running and port forwarding set up?")
        print(f"        Try: hdc forward tcp:{port} tcp:{port}")
        return False
    except socket.timeout:
        print(f"[ERROR] Connection timed out.")
        return False
    except Exception as e:
        print(f"[ERROR] {e}")
        return False


def setup_forward(port):
    """Setup hdc port forwarding."""
    try:
        cmd = f"hdc forward tcp:{port} tcp:{port}"
        print(f"[INFO] Running: {cmd}")
        result = subprocess.run(cmd, shell=True, capture_output=True, text=True, timeout=10)
        if result.returncode == 0:
            print(f"[OK] Port forwarding established: tcp:{port} -> tcp:{port}")
            return True
        else:
            print(f"[ERROR] hdc forward failed: {result.stderr.strip()}")
            return False
    except FileNotFoundError:
        print("[ERROR] hdc not found. Make sure it's in your PATH.")
        return False
    except subprocess.TimeoutExpired:
        print("[ERROR] hdc forward timed out.")
        return False


def interactive_mode(host, port):
    """Interactive mode for sending commands."""
    print(f"GM Command Sender - Interactive Mode")
    print(f"Connected to {host}:{port}")
    print(f"Type commands to send. 'quit' or 'exit' to leave.\n")
    while True:
        try:
            cmd = input("gm> ").strip()
        except (EOFError, KeyboardInterrupt):
            print("\nBye.")
            break
        if not cmd:
            continue
        if cmd.lower() in ("quit", "exit"):
            print("Bye.")
            break
        send_command(host, port, cmd)


def send_file(host, port, filepath):
    """Send commands from a file, one per line."""
    if not os.path.isfile(filepath):
        print(f"[ERROR] File not found: {filepath}")
        return
    with open(filepath, "r", encoding="utf-8") as f:
        lines = f.readlines()
    count = 0
    for line in lines:
        cmd = line.strip()
        if cmd and not cmd.startswith("#"):
            if send_command(host, port, cmd):
                count += 1
    print(f"\n[DONE] Sent {count} command(s).")


def main():
    parser = argparse.ArgumentParser(description="GM Command Sender for HarmonyOS devices")
    parser.add_argument("command", nargs="?", help="GM command to send")
    parser.add_argument("-H", "--host", default=DEFAULT_HOST, help=f"Target host (default: {DEFAULT_HOST})")
    parser.add_argument("-p", "--port", type=int, default=DEFAULT_PORT, help=f"Target port (default: {DEFAULT_PORT})")
    parser.add_argument("-i", "--interactive", action="store_true", help="Interactive mode")
    parser.add_argument("-f", "--file", help="Send commands from a file (one per line)")
    parser.add_argument("--setup", action="store_true", help="Auto setup hdc port forwarding before sending")
    args = parser.parse_args()

    if args.setup:
        if not setup_forward(args.port):
            sys.exit(1)

    if args.interactive:
        interactive_mode(args.host, args.port)
    elif args.file:
        send_file(args.host, args.port, args.file)
    elif args.command:
        if not send_command(args.host, args.port, args.command):
            sys.exit(1)
    else:
        parser.print_help()
        sys.exit(1)


if __name__ == "__main__":
    main()
