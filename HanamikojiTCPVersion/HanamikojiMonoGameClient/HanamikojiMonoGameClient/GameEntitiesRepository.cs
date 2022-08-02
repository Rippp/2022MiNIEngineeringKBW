using System;
using System.Collections.Generic;
using System.Linq;
using HanamikojiMonoGameClient.GameEntities;
using Microsoft.Xna.Framework;

namespace HanamikojiMonoGameClient;

public class GameEntitiesRepository : IEntitiesRepository
{
    private IDictionary<Guid, GiftCardEntity> _giftCardEntityDictionary = new Dictionary<Guid, GiftCardEntity>();
    private List<GameEntity> _gameEntities = new List<GameEntity>();
    private Button _submitButton;

    public GiftCardEntity GetByCardId(Guid cardId) => _giftCardEntityDictionary[cardId];

    public void Update(GameTime gameTime)
    {
        _gameEntities.ForEach(x => x.Update(gameTime));
        SortGameEntitiesByDrawOrder();
    }

    public void Add(GameEntity entity)
    {
        _gameEntities.Add(entity);
        if (entity is GiftCardEntity giftCardEntity)
        {
            _giftCardEntityDictionary.Add(giftCardEntity.CardId, giftCardEntity);
        }
    } 

    public IReadOnlyList<GameEntity> GetAll() => _gameEntities;

    public IReadOnlyList<GiftCardEntity> GetAllGiftCards() => _giftCardEntityDictionary.Values.ToList();

    public Button GetSubmitButton() => _submitButton;

    public void SetSubmitButtonEntity(Button button)
    {
        _gameEntities.Add(button);
        _submitButton = button;
    }

    private void SortGameEntitiesByDrawOrder() => _gameEntities.Sort((x, y) => x.DrawOrder.CompareTo(y.DrawOrder));
}

public interface IEntitiesRepository
{
    public GiftCardEntity GetByCardId(Guid cardId);

    public void Update(GameTime gameTime);

    public void Add(GameEntity entity);

    public IReadOnlyList<GameEntity> GetAll();

    public IReadOnlyList<GiftCardEntity> GetAllGiftCards();

    public Button GetSubmitButton();

    public void SetSubmitButtonEntity(Button button);
}