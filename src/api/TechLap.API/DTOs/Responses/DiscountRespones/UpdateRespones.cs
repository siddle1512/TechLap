namespace TechLap.API.DTOs.Responses.DiscountRespones;

public class UpdateRespones<T>
{
    // Trạng thái của phản hồi (thành công hoặc thất bại)
    public bool IsSuccess { get; set; }

    // Tin nhắn phản hồi từ API, chứa lỗi hoặc thông báo thành công
    public string Message { get; set; }

    // Dữ liệu phản hồi, có thể là bất kỳ kiểu nào (sử dụng generic T)
    public T Data { get; set; }
}