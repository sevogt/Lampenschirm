import socket

# Create a UDP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# Bind the socket to the port
server_address = ('localhost', 54322)
sock.bind(server_address)

while True:
    data, address = sock.recvfrom(4096)
    print(data)

sock.close()
