import { useContext, type FC } from "react";
import './SearchBoxComponent.scss';
import { SearchContext } from "../../context/SearchContext";

interface SearchProp {
    placeholder: string;
}

const SearchBoxComponent: FC<SearchProp> = (prop: SearchProp) => {
    const { placeholder } = prop;
    const { 
        searchBoxChangeHandler,
        searchProductHandler,
        searchBoxKeyDownHandler,
        query,
        suggestedProducts
     } = useContext(SearchContext);

    return (
        <>
            <div className="w-100">
                <input type="text" placeholder={placeholder} value={query} 
                    onChange={searchBoxChangeHandler} onKeyDown={searchBoxKeyDownHandler} />
                <div className="search-box__suggester">
                    {
                        suggestedProducts.map(suggest => {
                            return (
                                <p key={suggest} className="autocomplete__item" onClick={searchProductHandler}>
                                    <span className="autocomplete__icon loupe"></span>
                                    <span>{suggest}</span>
                                </p>
                            )
                        })
                    }
                </div>
            </div>
            
            <button className="search-box__btn-clear" onClick={() => searchBoxChangeHandler({ target: { value: '' } } as React.ChangeEvent<HTMLInputElement>)}>&times;</button>
        </>
    );
}

export default SearchBoxComponent;