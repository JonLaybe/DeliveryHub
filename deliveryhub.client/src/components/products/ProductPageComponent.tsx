import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getProductByIdAsync } from "../../services/catalog-service/ProductService";
import './ProductPageComponent.scss';
import type { ProductDto } from "../../models/catalog-service/ProductDto";
import { CATALOG_BASE_URL } from "../../constants/EndpointConstants";
import { useNavigate } from "react-router-dom";
import { createConversationAsync, getUserConversationsAsync } from "../../services/chat-service/ChatService";
import { addGroceryBasket, isProductInGroceryBasket } from "../../services/grocery-basket/GroceryBasketService";

const ProductPageComponent = () => {
    const { id } = useParams();
    const [product, setProduct] = useState<ProductDto | null>(null);
    const [isCreatingChat, setIsCreatingChat] = useState(false);
    const [isProductInTheBasket, setIsProductInTheBasket] = useState(isProductInGroceryBasket(id || ''));
    const [selectedTypeId, setSelectedTypeId] = useState(0);
    const [mainPreview, setMainPreview] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        if (!id) return;

        getProductByIdAsync(id)
            .then(data => {
                if (!data)
                    return;

                setProduct(data);

                if (data?.images) {
                    const imgMain = data.images.filter(x => x.type === 0)[0].url;
                    setMainPreview(imgMain);
                }
            })
            .catch(error => {
                console.error('Error fetching product details:', error);
            });
    }, [id]);

    const handleAddToBasket = () => {
        if (isProductInTheBasket) {
            navigate('/grocery_basket');
        }
        else {
            setIsProductInTheBasket(true);
            addGroceryBasket(product!)
        }
    }

    const handleWriteSeller = async () => {
        if (!product?.id) return;

        setIsCreatingChat(true);
        try {
            const conversationId = await createConversationAsync(product.id.toString());

            const conversations = await getUserConversationsAsync();
            const newConversation = conversations.find(c => c.id === conversationId);

            navigate(`/chat/${conversationId}`, {
                state: {
                    productName: product?.name,
                    conversationName: newConversation?.name || "Продавец",
                    conversationId: conversationId
                }
            });
        } catch (err) {
            console.error("Create conversation error:", err);
        } finally {
            setIsCreatingChat(false);
        }
    };

    const handelSelectedPreview = (data: any) => {
        setSelectedTypeId(data.type);
        setMainPreview(data.url);
    }

    return (
        <div className="product-page">
            <div className="product-nav">

            </div>
            <div className="product-content">
                <div className="media">
                    <div className="media__hidden-preview">
                        {
                            product?.images?.map((data, index) => (
                                <div className={`hidden-preview__product-image ${selectedTypeId === data.type ? 'preview_selected' : 'preview_no_selected'}`} key={index}>
                                    <img
                                        src={`${CATALOG_BASE_URL}${data.url}`}
                                        onClick={() => handelSelectedPreview(data)}
                                        alt={product?.name} />
                                </div>
                            ))
                        }
                    </div>
                    <div className="media__preview">
                        <img className="product-image" src={`${CATALOG_BASE_URL}${mainPreview}`} alt={product?.name} />
                    </div>
                </div>
                <div className="product-details">
                    <div className="product-description">{product?.description}</div>
                    {
                        Object.entries(product?.attributes || {}).map(([key, value]) => {
                            return (
                                <div className="product-attribute" key={key}>
                                    <span className="attribute-name">{key}:</span>
                                    <span className="attribute-value">{value}</span>
                                </div>
                            );
                        })
                    }
                </div>
                <div className="product-buy">
                    <div className="price-block">
                        <div className="img-price">
                            <svg width="100%" height="100%" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path fillRule="evenodd" clipRule="evenodd" d="M11.2652 2.69516C10.6645 2.42465 10.1595 2.19721 9.73068 2.04459C9.28214 1.88493 8.8365 1.77616 8.35628 1.79762C7.62342 1.83038 6.91938 2.09259 6.34363 2.54719C5.96636 2.84508 5.70043 3.21886 5.4656 3.63303C5.24111 4.02895 5.00787 4.53135 4.73045 5.1289L4.27271 6.1148C4.03305 6.16272 3.82694 6.23064 3.63803 6.3269C3.07354 6.61452 2.6146 7.07346 2.32698 7.63795C2 8.27968 2 9.11976 2 10.7999V15.1999C2 16.8801 2 17.7202 2.32698 18.3619C2.6146 18.9264 3.07354 19.3853 3.63803 19.6729C4.27976 19.9999 5.11984 19.9999 6.8 19.9999H18.8C19.9201 19.9999 20.4802 19.9999 20.908 19.7819C21.2843 19.5902 21.5903 19.2842 21.782 18.9079C22 18.4801 22 17.92 22 16.7999V13.9999H17C16.4477 13.9999 16 13.5522 16 12.9999C16 12.4476 16.4477 11.9999 17 11.9999H22V9.19992C22 8.07982 22 7.51976 21.782 7.09194C21.5903 6.71562 21.2843 6.40965 20.908 6.21791C20.4802 5.99992 19.9201 5.99992 18.8 5.99992H18.6044L11.2652 2.69516ZM13.7333 5.99992L10.4804 4.53519C9.8338 4.24401 9.40304 4.05089 9.06001 3.92879C8.7281 3.81065 8.56039 3.79049 8.44559 3.79563C8.13151 3.80967 7.82977 3.92204 7.58302 4.11687C7.49284 4.18808 7.37916 4.31303 7.20539 4.6195C7.02648 4.93503 6.82783 5.36082 6.53107 5.99997C6.61826 5.99992 6.70787 5.99992 6.8 5.99992H13.7333Z" fill="rgb(250, 31, 75)"></path></svg>
                        </div>
                        <span className="product-price">{product?.price}  ₽</span>
                        <div className="product-remain">Осталось {product?.availableQty} шт.</div>
                    </div>
                    <div className="action-block">
                        <button className="default-button" onClick={handleAddToBasket} disabled={(product?.availableQty ?? 0) <= 0}>
                            {isProductInTheBasket ? "В корзине" : "Добавить в корзину"}
                        </button>
                        <button className="default-button" onClick={handleWriteSeller} disabled={isCreatingChat}>
                            {isCreatingChat ? "Создание чата..." : "Написать продавцу"}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ProductPageComponent;