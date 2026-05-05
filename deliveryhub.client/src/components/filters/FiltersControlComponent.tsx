import { useContext, useState, type FC } from "react";
import './FiltersControlComponent.scss';
import { SearchContext } from "../../context/SearchContext";
import SortingFilterComponent from "./SortingFilterComponent";
import CategoriesFilterComponent from "./CategoriesFilterComponent";
import FiltersComponent from "./FiltersComponent";
import type { UUIDTypes } from "uuid";

const FiltersControlComponent: FC = () => {
     const { 
            showFilters,
            filtersCount,
        } = useContext(SearchContext);

    const [selectedSort, setSelectedSort] = useState<string>('PriceAsc');
    const [selectedCategoryId, setSelectedCategoryId] = useState<UUIDTypes | undefined>(undefined);

    return (
        <>
            <div className="filters-block">
                <SortingFilterComponent selectedSort={selectedSort} setSelectedSort={setSelectedSort} />

                <button onClick={() => showFilters(true)} className="filter-button filter-button--all">Фильтры</button>
                {filtersCount > 0 && <span className="filters-count">{filtersCount}</span>}

                <CategoriesFilterComponent selectedCategoryId={selectedCategoryId} setSelectedCategoryId={setSelectedCategoryId} />
            </div>
            <FiltersComponent selectedSort={selectedSort} selectedCategoryId={selectedCategoryId} />
        </>
    );
}

export default FiltersControlComponent;