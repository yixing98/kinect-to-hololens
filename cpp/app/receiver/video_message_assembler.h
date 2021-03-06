#pragma once

#include <map>
#include "video_renderer_state.h"

namespace kh
{
class VideoMessageAssembler
{
public:
    VideoMessageAssembler(const int session_id, const asio::ip::udp::endpoint remote_endpoint);
    void assemble(UdpSocket& udp_socket,
                  std::vector<VideoSenderPacketData>& video_packet_data_vector,
                  std::vector<ParitySenderPacketData>& parity_packet_data_vector,
                  VideoRendererState video_renderer_state,
                  std::map<int, VideoSenderMessageData>& video_frame_messages);

private:
    const int session_id_;
    const asio::ip::udp::endpoint remote_endpoint_;
    std::unordered_map<int, std::vector<std::optional<VideoSenderPacketData>>> video_packet_collections_;
    std::unordered_map<int, std::vector<std::optional<ParitySenderPacketData>>> parity_packet_collections_;
};
}