#pragma once

#include <asio.hpp>
#include <optional>
#include "k4a/k4a.hpp"

namespace kh
{
class SenderSocket
{
public:
    SenderSocket(asio::ip::udp::socket&& socket, asio::ip::udp::endpoint remote_endpoint, int send_buffer_size);
    void sendInitPacket(int session_id, k4a_calibration_t calibration, std::error_code& error);
    void sendAudioPacket(int session_id, int frame_id, std::vector<uint8_t>& opus_frame, int opus_frame_size, std::error_code& error);
    std::optional<std::vector<std::byte>> receive(std::error_code& error);
    static std::vector<std::byte> createFrameMessage(float frame_time_stamp, bool keyframe, std::vector<std::byte>& vp8_frame,
                                                   std::byte* depth_encoder_frame, uint32_t depth_encoder_frame_size);
    static std::vector<std::vector<std::byte>> createFramePackets(int session_id, int frame_id, const std::vector<std::byte>& frame_message);
    static std::vector<std::vector<std::byte>> createXorPackets(int session_id, int frame_id,
                                                              const std::vector<std::vector<std::byte>>& packets, int max_group_size);
    void sendPacket(const std::vector<std::byte>& packet, std::error_code& error);

private:
    asio::ip::udp::socket socket_;
    asio::ip::udp::endpoint remote_endpoint_;
};
}