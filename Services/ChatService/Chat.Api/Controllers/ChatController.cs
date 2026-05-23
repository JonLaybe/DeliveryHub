using Chat.Application.DTOs;
using Chat.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Controllers
{
    /// <summary>
    /// Управляет диалогами между покупателями и продавцами.
    /// </summary>
    /// <remarks>
    /// Контроллер позволяет создавать новые диалоги и получать список
    /// диалогов текущего аутентифицированного пользователя.
    /// Идентификатор пользователя извлекается из JWT-токена (claim <c>sub</c>).
    /// </remarks>
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;
        private readonly IValidator<CreateConversationRequest> _validator;

        public ChatController(
            IConversationService conversationService,
            IMessageService messageService,
            IValidator<CreateConversationRequest> validator)
        {
            _conversationService = conversationService;
            _messageService = messageService;
            _validator = validator;
        }

        /// <summary>
        /// Создаёт новый диалог между покупателем и продавцом.
        /// </summary>
        /// <remarks>
        /// Покупатель определяется автоматически на основе JWT-токена.
        /// Если диалог между этими пользователями уже существует,
        /// сервис может вернуть существующий идентификатор.
        /// </remarks>
        /// <param name="request">
        /// Данные для создания диалога:
        /// идентификатор продавца.
        /// </param>
        /// <returns>
        /// Идентификатор созданного (или существующего) диалога.
        /// </returns>
        /// <response code="200">Диалог успешно создан</response>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="401">Пользователь не аутентифицирован</response>
        [HttpPost("conversation")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request)
        {

            _validator.ValidateAndThrow(request);
            // Взять buyerId из JWT потом
            var buyerId = Guid.Parse(request.BuyerId);
            var sellerId = Guid.Parse(request.SellerId);

            var conversationId = await _conversationService.CreateConversationAsync(buyerId, sellerId);

            return Ok(conversationId);
        }

        /// <summary>
        /// Возвращает список диалогов текущего пользователя.
        /// </summary>
        /// <remarks>
        /// Пользователь определяется на основе JWT-токена.
        /// Пока как заглушка используется id из тела запроса
        /// В список входят все диалоги, в которых пользователь является
        /// покупателем или продавцом.
        /// </remarks>
        /// <returns>
        /// Коллекция диалогов пользователя.
        /// </returns>
        /// <response code="200">Список диалогов успешно получен</response>
        /// <response code="401">Пользователь не аутентифицирован</response>
        [HttpGet("conversation/{userId:guid}")]
        public async Task<IActionResult> GetMyConversations(Guid userId)
        {
            var conversations = await _conversationService.GetUserConversationsAsync(userId);

            return Ok(conversations);
        }

        /// <summary>
        /// Получает все сообщения для указанного диалога (conversation).
        /// </summary>
        /// <param name="conversationId">Идентификатор диалога (ConversationId).</param>
        /// <returns>Список сообщений в формате MessageDto для данного диалога.</returns>
        /// <response code="200">Возвращает список сообщений для указанного диалога.</response>
        [HttpGet("conversation/{conversationId:guid}/messages")]
        public async Task<IActionResult> GetMessages(Guid conversationId)
        {
            var messages = await _messageService.GetMessagesAsync(conversationId);

            return Ok(messages);
        }
    }
}
