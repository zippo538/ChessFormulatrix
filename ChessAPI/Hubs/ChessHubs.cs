using Microsoft.AspNetCore.SignalR;
using ChessAPI.DTO;
namespace ChessAPI.Hubs;

public class ChessHubs : Hub
{
    // 1. Klien (React) memanggil metode ini saat masuk ke halaman permainan
    public async Task JoinGame(string gameId)
    {
        // Memasukkan koneksi klien ke dalam "Grup" berdasarkan ID Permainan
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            
        // Opsional: Memberitahu pemain lain di room bahwa ada yang bergabung
        await Clients.OthersInGroup(gameId).SendAsync("PlayerJoined");
    }

    // 2. Klien memanggil metode ini setiap kali memindahkan bidak di UI
    public async Task MakeMove(string gameId, MoveDTO move)
    {
        // TODO: Panggil BoardService / MovementService di sini untuk validasi langkah.
        // Misal: bool isValid = MovementService.MoveIsValid(board, move);
        // Jika tidak valid, Anda bisa me-return atau mengirim error ke klien.

        // Jika langkah valid, teruskan (broadcast) data pergerakan tersebut ke pemain lawan.
        // Kita menggunakan 'OthersInGroup' agar pemain yang mengirim langkah 
        // tidak menerima notifikasi pergerakannya sendiri.
        await Clients.OthersInGroup(gameId).SendAsync("OnMoveReceived", move);
    }

    // 3. Klien bisa memanggil ini saat menyerah atau keluar
    public async Task LeaveGame(string gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        await Clients.OthersInGroup(gameId).SendAsync("OpponentLeft");
    }
}