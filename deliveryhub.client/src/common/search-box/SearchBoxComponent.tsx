import type { FC } from "react";
import './SearchBoxComponent.scss';

interface SearchProp {
    placeholder: string;
}

const SearchBoxComponent: FC<SearchProp> = (prop: SearchProp) => {
    const { placeholder } = prop;

    return (
        <>
            <input type="text" placeholder={placeholder} />
        </>
    );
}

export default SearchBoxComponent;