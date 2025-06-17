using IssueTracker.Domain.Interfaces;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.DTOs;
using AutoMapper;

namespace IssueTracker.Application.Services
{
    public class CardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IMapper _mapper;

        public CardService(ICardRepository cardRepository, IMapper mapper)
        {
            _cardRepository = cardRepository;
            _mapper = mapper;
        }

        public async Task<Card?> GetCardByIdAsync(int id)
        {
            return await _cardRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Card>> GetAllCardsAsync()
        {
            return await _cardRepository.GetAllAsync();
        }

        public async Task<Card> AddCardAsync(CreateCardDTO cardDto)
        {
            var card = _mapper.Map<Card>(cardDto);
            await _cardRepository.AddAsync(card);
            return card;
        }

        public async Task<Card> UpdateCardAsync(UpdateCardDTO cardDto)
        {
            var card = await _cardRepository.GetByIdAsync(cardDto.Id);
            if (card != null)
            {
                card.Title = cardDto.Title;
                card.Description = cardDto.Description;
                card.ItemId = cardDto.ItemId;                
                card.Labels = cardDto.Labels;
                await _cardRepository.UpdateAsync(card);
            }
            return card;
        }

        public async Task DeleteCardAsync(int id)
        {
            await _cardRepository.DeleteAsync(id);
        }
    }
}
