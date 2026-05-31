using Chat.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Controllers
{
    /// <summary>
    /// Управляет диалогами между покупателями и продавцами.
    /// </summary>
    /// <remarks>
    /// Контроллер позволяет создавать новые диалоги и получать список
    /// диалогов текущего аутентифицированного пользователя.
    /// </remarks>
    [Authorize]
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;

        public ChatController(
            IConversationService conversationService,
            IMessageService messageService)
        {
            _conversationService = conversationService;
            _messageService = messageService;
        }

        /// <summary>
        /// Создаёт новый диалог между покупателем и продавцом.
        /// </summary>
        /// <remarks>
        /// Покупатель определяется автоматически на основе JWT-токена.
        /// Если диалог между этими пользователями уже существует,
        /// сервис может вернуть существующий идентификатор.
        /// </remarks>
        /// <param name="productId">Идентификатор товара</param>
        /// <returns>Идентификатор созданного (или существующего) диалога</returns>
        /// <response code="200">Диалог успешно создан</response>
        /// <response code="401">Пользователь не аутентифицирован</response>
        [HttpPost("conversation/{productId:guid}")]
        public async Task<IActionResult> CreateConversation(Guid productId)
        {
            var currentUser = GetUserGuid();

            var conversationId = await _conversationService.CreateConversationAsync(currentUser, productId);

            return Ok(conversationId);
        }

        /// <summary>
        /// Возвращает список диалогов текущего пользователя.
        /// </summary>
        /// <remarks>
        /// Пользователь определяется на основе JWT-токена.
        /// В список входят все диалоги, в которых пользователь является
        /// покупателем или продавцом.
        /// </remarks>
        /// <returns>Коллекция диалогов пользователя</returns>
        /// <response code="200">Список диалогов успешно получен</response>
        /// <response code="401">Пользователь не аутентифицирован</response>
        [HttpGet("conversation")]
        public async Task<IActionResult> GetMyConversations()
        {
            var currentUser = GetUserGuid();

            var conversations = await _conversationService.GetUserConversationsAsync(currentUser);

            return Ok(conversations);
        }

        /// <summary>
        /// Получает все сообщения для указанного диалога.
        /// </summary>
        /// <param name="conversationId">Идентификатор диалога</param>
        /// <returns>Список сообщений для данного диалога</returns>
        /// <response code="200">Возвращает список сообщений для указанного диалога</response>
        /// <response code="401">Пользователь не аутентифицирован</response>
        [HttpGet("conversation/{conversationId:guid}/messages")]
        public async Task<IActionResult> GetMessages(Guid conversationId)
        {
            var currentUser = GetUserGuid();

            var messages = await _messageService.GetMessagesAsync(conversationId, currentUser);

            return Ok(messages);
        }

        private Guid GetUserGuid()
        {
            var userId = User.FindFirst("uid")?.Value;
            var currentUser = new Guid(userId!);
            return currentUser;
        }
    }
}
