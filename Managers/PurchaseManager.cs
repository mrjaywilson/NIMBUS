using UnityEngine;
using UnityEngine.Purchasing;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles purchasing items from various stores.
/// </summary>
public class PurchaseManager : MonoBehaviour, IStoreListener
{
    private static IStoreController _storeController;
    private static IExtensionProvider _storeExtensionProvider;

    // List of product consts
    public const string PRODUCT_ID_CONSUMABLE = "consumable";
    public const string PRODUCT_ID_NONCONSUMABLE = "nonconsumable";
    public const string PRODUCT_ID_SUBSCRIPTION = "subscription";
    private const string PRODUCT_NAME_APPLE_SUBSCRIPTION = "com.darkhorizonstudio.subscription.new";
    private const string PRODUCT_NAME_GOOGLEPLAY_SUBSCRIPTION = "com.darkhorizonstudio.subscription.original";

    /// <summary>
    /// Method called when the object is instantiated.
    /// </summary>
    void Start()
    {
        if (_storeController == null)
        {
            InitPurchasing();
        }
    }

    /// <summary>
    /// Initialization of a purchase.
    /// </summary>
    public void InitPurchasing()
    {
        // If already initialized, don't do it again.
        if (IsInitialized())
        {
            return;
        }

        // Add product to instialization instance
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(PRODUCT_ID_CONSUMABLE, ProductType.Consumable);
        builder.AddProduct(PRODUCT_ID_NONCONSUMABLE, ProductType.NonConsumable);
        builder.AddProduct(PRODUCT_ID_SUBSCRIPTION, ProductType.Subscription, new IDs
        {
            {
                PRODUCT_NAME_APPLE_SUBSCRIPTION, AppleAppStore.Name
            },
            {
                PRODUCT_NAME_GOOGLEPLAY_SUBSCRIPTION, GooglePlay.Name
            }
        });

        // Initialize started
        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    /// Check of the puchasing is initialized
    /// </summary>
    private bool IsInitialized()
    {
        return _storeController != null && _storeExtensionProvider != null;
    }

    /// <summary>
    /// Method called to prurchase an item
    /// </summary>
    public void BuyConsumable()
    {
        BuyProductID(PRODUCT_ID_CONSUMABLE);
    }

    /// <summary>
    /// Method called to prurchase an item
    /// </summary>
    public void BuyNonConsumable()
    {
        BuyProductID(PRODUCT_ID_NONCONSUMABLE);
    }

    /// <summary>
    /// Method called to prurchase an item
    /// </summary>
    public void BuySubscription()
    {
        BuyProductID(PRODUCT_ID_SUBSCRIPTION);
    }

    /// <summary>
    /// Method used to connect and purchase an item.
    /// </summary>
    /// <param name="productID">String: The item to purchase.</param>
    private void BuyProductID(string productID)
    {
        if (IsInitialized())
        {
            Product product = _storeController.products.WithID(productID);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log($"Purchase product asynchronously: {product.definition.id}");
                _storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log($"BuyProductID(): FAIL. Not a purchasable product, either is not found or not available.");
            }
        }
        else
        {
            Debug.Log($"BuyProductID(): FAIL. Not Initialized.");
        }
    }

    /// <summary>
    /// Restore purchase to device if lost current purchases for some reason: Apple Platforms Only
    /// </summary>
    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log($"RestorePruchases(): FAIL. Not Initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log($"RestorePurchases(): STARTED.");

            var apple = _storeExtensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions((result) =>
            {
                Debug.Log($"RestorePurchases(): CONTINUING -> {result}. If no further messages, no purchase available to restore.");
            });
        }
        else
        {
            Debug.Log($"RestorePurchases(): FAIL. Not supported on this platform. Current Platform -> {Application.platform}");
        }
    }

    /// <summary>
    /// Called when initialization succeeds.
    /// </summary>
    /// <param name="controller">Pass the controller service.</param>
    /// <param name="extensions">Pass the Extension service.</param>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {

        Debug.Log("OnInitialized(): PASS.");

        _storeController = controller;
        _storeExtensionProvider = extensions;

    }

    /// <summary>
    /// Method called of initialization failed.
    /// </summary>
    /// <param name="error">Error data.</param>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"OnInitializeFailed(): Initialization Failure Reason: {error}");
    }

    /// <summary>
    /// Called of a purchase fails.
    /// </summary>
    /// <param name="product">Product that failed.</param>
    /// <param name="failureReason">Reason for failure.</param>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"OnPurchaseFailed(): Product -> {product.definition.storeSpecificId} | Failure Reason: {failureReason}");
    }

    /// <summary>
    /// Called after purchase attempt.
    /// </summary>
    /// <param name="purchaseEvent">Event purchase data.</param>
    /// <returns></returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (string.Equals(purchaseEvent.purchasedProduct.definition.id, PRODUCT_ID_CONSUMABLE, System.StringComparison.Ordinal))
        {
            Debug.Log($"ProcessPurchase(): PASS. Product -> {purchaseEvent.purchasedProduct.definition.id}");

            // Do consumable
        }
        else if (string.Equals(purchaseEvent.purchasedProduct.definition.id, PRODUCT_ID_NONCONSUMABLE, System.StringComparison.Ordinal))
        {
            Debug.Log($"ProcessPurchase(): PASS. Product -> {purchaseEvent.purchasedProduct.definition.id}");

            // Do non-consumable
        }
        else if (string.Equals(purchaseEvent.purchasedProduct.definition.id, PRODUCT_ID_SUBSCRIPTION, System.StringComparison.Ordinal))
        {
            Debug.Log($"ProcessPurchase(): PASS. Product -> {purchaseEvent.purchasedProduct.definition.id}");

            // Do subscription
        }
        else
        {
            Debug.Log($"ProcessPurchase: FAIL. Unrecongized Product -> {purchaseEvent.purchasedProduct.definition.id}");
        }

        return PurchaseProcessingResult.Complete;
    }
}
