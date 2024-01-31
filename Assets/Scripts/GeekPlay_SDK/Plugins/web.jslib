var plugin = {

  /////MAIN/////
  GamePlatform : function()
    {
        console.log(location.hostname);
        p = UTF8ToString(location.hostname);
        myGameInstance.SendMessage('Init', 'ChangePlatform', p);
    },
  /////MAIN/////


  /////YANDEX//////
    GameReady : function()
    {
        ysdk.features.LoadingAPI.ready();
    },

    IsMobile : function()
    {
        if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
            myGameInstance.SendMessage('Init', 'ItIsMobile');
        }
    },

    RateGame: function () {
        ysdk.feedback.canReview()
        .then(({ value, reason }) => {
            if (value) {
                ysdk.feedback.requestReview()
                    .then(({ feedbackSent }) => {
                        console.log(feedbackSent);
                    })
            } else {
                console.log(reason)
            }
        })
    },

    SaveExtern: function(date) {
        var dateString = UTF8ToString(date);
        var myobj = JSON.parse(dateString);
        player.setData(myobj);
      },

    LoadExtern: function(){
        player.getData().then(_date => {
            //const myJSON = JSON.stringify(_date);
            const myJSON = UTF8ToString(_date);
            myGameInstance.SendMessage('Init', 'SetPlayerData', myJSON);
        });
    },  	
    
    AdInterstitial : function () {
        ysdk.adv.showFullscreenAdv({
          callbacks: {
        onOpen: function(wasShown) {
          myGameInstance.SendMessage('Init', 'StopMusAndGame');
        },
        onClose: function(wasShown) {
          myGameInstance.SendMessage('Init', 'ResumeMusAndGame');
        },
        onError: function(error) {
          // some action on error
        }
        }
    });
    },

    AdReward : function(){
        ysdk.adv.showRewardedVideo({
        callbacks: {
        onOpen: () => {
          console.log('Video ad open.');
          myGameInstance.SendMessage('Init', 'StopMusAndGame');
        },
        onRewarded: () => {
          //myGameInstance.SendMessage('Init', 'OnRewarded');
        },
        onClose: () => {
          console.log('Video ad closed.');
          myGameInstance.SendMessage('Init', 'OnRewarded');
          myGameInstance.SendMessage('Init', 'ResumeMusAndGame');
        }, 
        onError: (e) => {
          console.log('Error while open video ad:', e);
        }
    }
    });
    },

    GetLang : function(){
      var lang = ysdk.environment.i18n.lang;
      var bufferSize = lengthBytesUTF8(lang) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(lang, buffer, bufferSize);
      return buffer;
    }, 

    SetToLeaderboard : function(value, leaderboardName){
          leaderboardName = UTF8ToString(leaderboardName);
          ysdk.getLeaderboards()
            .then(lb => {
          // Без extraData
          lb.setLeaderboardScore(leaderboardName, value);
          });
    },

    GetDomain: function() {
      var lang = ysdk.environment.i18n.tld;
      var bufferSize = lengthBytesUTF8(lang) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(lang, buffer, bufferSize);
      return buffer;
    },

    BuyItem : function (idOrTag, d) {
    	idOrTag = UTF8ToString(idOrTag);
      var dateString = UTF8ToString(d);
      var myobj = JSON.parse(dateString);
      player.setData(myobj);
    	ysdk.getPayments({ signed: true }).then(_payments => {
        	payments = _payments;
        	payments.purchase(idOrTag).then(purchase => {
        		myGameInstance.SendMessage('Init', 'OnPurchasedItem');
        		payments.consumePurchase(purchase.purchaseToken); //для разовых покупок
        		window.focus();
    		})
	    	}).catch(err => {
	        	console.alert(err);
	   	}).catch(err => {
	        console.alert(err);
	    })
    },

    CheckBuyItem: function (idOrTag) {
      	idOrTag = UTF8ToString(idOrTag);
        console.log(idOrTag);

        payments.getPurchases().then(purchases => purchases.forEach(consumePurchase));


  		  payments.getPurchases().then(purchases => {
  		    if (purchases.some(purchase => purchase.productID === idOrTag)) {
  		    	myGameInstance.SendMessage('Init', 'SetPurchasedItem');
  		    }
  		  }).catch(err => {
  		    // Выбрасывает исключение USER_NOT_AUTHORIZED для неавторизованных пользователей.
  		  })
	  },

    GetLeaderboard: function (type, number) {
      type = UTF8ToString(type);
      console.log(type);

          ysdk.getLeaderboards()
      .then(lb => {
        // Получение 10 топов
        lb.getLeaderboardEntries('Points', { quantityTop: 10 })
          .then(res => {
            console.log(res);
            if (res.entries.length <= number)
            {
              console.log("NULL");
              return;
            }
            else if (type == "score")
            {
              console.log("SCORE");              
              console.log(String(res.entries[number].score));
              myGameInstance.SendMessage('Init', 'GetLeaders', String(res.entries[number].score));
              //return String(res.entries[number].score);
            }
            else if (type == "name")
            {
              console.log("NAME");
              myGameInstance.SendMessage('Init', 'GetLeadersName', String(res.entries[number].player.publicName));
              //return UTF8ToString(res.entries[number].player.publicName);
            }
          });
      });
    },
  /////YANDEX//////



  /////VK GAMES//////
  //добавить игру в избранное
  VK_Star: function () {
    vkBridge.send('VKWebAppAddToFavorites')
    .then((data) => { 
      if (data.result) {
        // Мини-приложение или игра добавлены в избранное
      }
    })
    .catch((error) => {
      // Ошибка
      console.log(error);
    });
  },
  //поделиться игрой не стене
  VK_Share: function () {
    vkBridge.send('VKWebAppShare', {
      link: 'https://vk.com/vkappsdev'
    })
    .then((data) => { 
      if (data.result) {
        // Запись размещена
      }
    })
    .catch((error) => {
      // Ошибка
      console.log(error);
    });
  },
  //пригласить в игру
  VK_Invite: function () {
    vkBridge.send('VKWebAppShowInviteBox', {
      requestKey: "key-12345" //  Ключ приглашения
      })
      .then( (data) => {
        if (data.success) {
          // Пользователь нажал «Пригласить» 
          // ...
      
          // Этим выбранным пользователям 
          // не удалось отправить приглашения 
          console.log('Приглашения не отправлены', data.notSentIds);
        }
      })
      .catch( (error) => {
        console.log(error); // Ошибка 
      });
  },
  //баннер
  VK_Banner: function () {
    vkBridge.send('VKWebAppShowBannerAd', {
      banner_location: 'bottom'
      })
     .then((data) => { 
        if (data.result) {
          // Баннерная реклама отобразилась
        }
      })
      .catch((error) => {
        // Ошибка
        console.log(error);
    });
  },
  //проверить, загрузился ли интерстишл
  VK_AdInterCheck: function () {
    vkBridge.send('VKWebAppCheckNativeAds', { ad_format: 'interstitial'});
  },
  //проверить, загрузился ли ревард
  VK_AdRewardCheck: function () {
    vkBridge.send('VKWebAppCheckNativeAds', { ad_format: 'reward' })
    .then((data) => {
      if (data.result) {
        // Предзагруженная реклама есть.
   
        // Теперь можно создать кнопку
        // "Прокачать героя за рекламу".   
        // ...
              
        } else {
          console.log('Рекламные материалы не найдены.');
        }
    })
    .catch((error) => { console.log(error); /* Ошибка */  });
  },
  //показать интерстишл
  VK_Interstitial: function () {
    // Показать рекламу
    vkBridge.send('VKWebAppShowNativeAds', { ad_format: 'interstitial' })
      .then((data) => {
        if (data.result)
          console.log('Реклама показана');
        else
          console.log('Ошибка при показе');
      })
      .catch((error) => { console.log(error); /* Ошибка */ });
  },
  //показать ревард
  VK_Rewarded: function () {
    // Показать рекламу
    vkBridge.send('VKWebAppShowNativeAds', { ad_format: 'reward' })
      .then((data) => {
        if (data.result) // Успех
        {
          myGameInstance.SendMessage('Init', 'OnRewarded');
          console.log('Реклама показана');
        }
        else // Ошибка 
        {
          console.log('Ошибка при показе');
        }
      })
      .catch((error) => { console.log(error); /* Ошибка */ });
  },
  //только на мобилках (VK Direct)
  VK_OpenLeaderboard: function (value) {
    vkBridge.send("VKWebAppShowLeaderBoardBox", {
    user_result: value
    })
    .then( (data) => {
      if (data.success) {
          // Диалоговое окно было показано
          // ...
      }
    })
    .catch( (error) => {
      console.log(error); // Ошибка
    });
  },
  //для сохранения и загрузки нужно в поле ключа просто передавать JSON 
  VK_Load: function () {
    vkBridge.send('VKWebAppStorageGet', {
    keys: [
      //массив названий ключей, значения которых хотим получить
      'PlayerData'
    ]})
    .then((data) => { 
      if (data.keys) {
        console.log("LOAD");
        console.log(JSON.stringify(data.keys[0]));
          //const myJSON = JSON.stringify(data.keys[0]);
          //console.log(myJSON);
          myGameInstance.SendMessage('Init', 'SetPlayerData', myJSON);//data.keys[0]);
        console.log("LOAD2");
      }
      else
      {
        var s = null;
        myGameInstance.SendMessage('Init', 'SetPlayerData', s);
      }
    })
    .catch((error) => {
      // Ошибка
      console.log("ERROR LOAD");
      console.log(error);
    });
  },
  //сохраняет значение переменной, переданной в метод
  VK_Save: function (saveData) {
    var dateString = UTF8ToString(saveData);
    vkBridge.send('VKWebAppStorageSet', {
     key: 'PlayerData',
     value: dateString
    })
    .then((data) => { 
      if (data.result) {
        console.log(dateString);
      }
    })
    .catch((error) => {
      // Ошибка
      console.log(error);
    });
  },
  //приглашение в сообщество
  VK_ToGroup: function () {
    vkBridge.send('VKWebAppJoinGroup', {
    group_id: 195607270
    })
    .then((data) => { 
      if (data.result) {
        myGameInstance.SendMessage('Init', 'OnRewarded');
      }
    })
    .catch((error) => {
      // Ошибка
      console.log(error);
    });
  },
  /////VK GAMES//////


  /////KONGREGATE/////
  Kongregate_InApp: function (itemName) {
      itemName = UTF8ToString(itemName);
      kongregate.mtx.purchaseItemsRemote(itemName, onPurchaseResult);
  },

  onPurchaseResult: function(result) {
        trace("Purchase success:" + result.success + ", id: " + result.item_order_id);
        myGameInstance.SendMessage('Init', 'OnPurchasedItem');
      },
  /////KONGREGATE/////


  /////OK/////
  OK_Interstitial: function() {
    FAPI.UI.showAd()
  },

  OK_ShowRating: function() {
    FAPI.UI.showRatingDialog();
  },

  OK_LoadRewardedAd: function() {
    FAPI.UI.loadAd();
  },

  OK_RequestBannerAds: function() {
    FAPI.invokeUIMethod("requestBannerAds")
  },

  OK_ShowRewardedAd: function() {
    FAPI.UI.showLoadedAd()
  },

  OK_ShowBannerAds: function() {
    FAPI.invokeUIMethod("showBannerAds", "top")
  },
  /////OK/////

};

mergeInto(LibraryManager.library, plugin);