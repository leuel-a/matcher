namespace PlayerService.Models;

public class PlayerState
{
    public required string PlayerId { set; get; }
    public required string CurrentRoomId { set; get; }
    public PlayerStatus Status { get; set; }
    public DateTime? RoomJoinTime { set; get; }
}
