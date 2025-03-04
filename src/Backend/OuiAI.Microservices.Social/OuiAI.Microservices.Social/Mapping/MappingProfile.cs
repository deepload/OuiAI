using AutoMapper;
using OuiAI.Microservices.Social.DTOs;
using OuiAI.Microservices.Social.Models;

namespace OuiAI.Microservices.Social.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // UserFollow mappings
            CreateMap<UserFollowModel, UserFollowDto>();
            CreateMap<UserFollowModel, FollowerDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.FollowerId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.FollowerUsername))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FollowerDisplayName))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.FollowerProfileImageUrl))
                .ForMember(dest => dest.FollowedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<UserFollowModel, FolloweeDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.FolloweeId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.FolloweeUsername))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FolloweeDisplayName))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.FolloweeProfileImageUrl))
                .ForMember(dest => dest.FollowedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // Notification mappings
            CreateMap<NotificationModel, NotificationDto>();
            CreateMap<CreateNotificationDto, NotificationModel>();

            // Activity mappings
            CreateMap<ActivityModel, ActivityDto>();
            CreateMap<CreateActivityDto, ActivityModel>();

            // Conversation mappings
            CreateMap<ConversationModel, ConversationDto>()
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants))
                .ForMember(dest => dest.LastMessage, opt => opt.Ignore());

            CreateMap<ConversationModel, ConversationDetailDto>()
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants))
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages));

            // ConversationParticipant mappings
            CreateMap<ConversationParticipantModel, ParticipantDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
                .ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(src => src.JoinedAt))
                .ForMember(dest => dest.LastReadAt, opt => opt.MapFrom(src => src.LastReadAt));

            // Message mappings
            CreateMap<MessageModel, MessageDto>()
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => new SenderDto
                {
                    UserId = src.SenderId,
                    Username = src.SenderUsername,
                    DisplayName = src.SenderDisplayName,
                    ProfileImageUrl = src.SenderProfileImageUrl
                }));
            
            CreateMap<CreateMessageDto, MessageModel>();
        }
    }
}
