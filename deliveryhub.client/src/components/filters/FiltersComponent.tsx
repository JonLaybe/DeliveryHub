import './FiltersComponent.scss';
import { useContext, useRef, useState } from "react";
import { SearchContext } from "../../context/SearchContext";
import { filterNamesMapping } from "../../constants/FilterNameValueMap";

const FiltersComponent = () => {
    const minPrice = useRef<HTMLInputElement>(null);
    const maxPrice = useRef<HTMLInputElement>(null);
    const { 
        serachProductsAndSetResults,
        filterAttributes, 
        filtersIsShown, 
        showFilters,
        setFiltersCount,
        filtersCount,
        query,
     } = useContext(SearchContext);

    const [selectedFilters, setSelectedFilters] = useState<{[key: string]: string[]}>({});

    const filterAttributeClickHandler = (filterName: string, filterValue: string) => {
        setSelectedFilters(prev => {
            const currentValues = prev[filterName] || [];

            const newValues = currentValues.includes(filterValue)
                ? currentValues.filter(v => v !== filterValue)
                : [...currentValues, filterValue];

            setFiltersCount(Object.values({ ...prev, [filterName]: newValues }).reduce(
                (acc, values) => acc + (values.length > 0 ? 1 : 0), 0
            ));

            return { ...prev, [filterName]: newValues };
        });
    }

    const clearFilters = () => {
        setSelectedFilters({});
        setFiltersCount(0);
    }

    const applyFilters = () => {
        showFilters(false);

        serachProductsAndSetResults({
            text: query,
            minPrice: minPrice.current?.value ? Number(minPrice.current.value) : undefined,
            maxPrice: maxPrice.current?.value ? Number(maxPrice.current.value) : undefined,
            attributes: selectedFilters,
        });
    }

    return (filtersIsShown &&
        <div className="filters">
            <div className='filters-container'>
                <div className="filters-header">
                    <h3>Фильтры</h3>
                    <button onClick={() => showFilters(false)} className="filters-dismiss" aria-label="Закрыть фильтр" type="button"></button>
                </div>
                <div className='filter-list'>
                    <div className='filter-item'>
                        <div className='filter-item-header'>
                            <h3>Цена, ₽</h3>
                        </div>
                        <div className='filter-item-body'>
                            <div className='filter-price'>
                                <div className='filter-price-item'>
                                    <h3 className='filter-price-title'>От</h3>
                                    <input ref={minPrice} className='filter-price-input' type="number"></input>
                                </div>
                                <div className='filter-price-item'>
                                    <h3 className='filter-price-title'>До</h3>
                                    <input ref={maxPrice} className='filter-price-input' type="number"></input>
                                </div>
                            </div>
                        </div>
                    </div>
                    {Object.entries(filterAttributes || {}).map(([filterName, values]) => (
                        <div className='filter-item' key={filterName}>
                            <div className='filter-item-header'>
                                <h3>{filterNamesMapping.Keys[filterName] ?? filterName}</h3>
                            </div>
                            <div className='filter-item-body'>
                                <div className='filter-checkbox-list'>
                                    {
                                        values.map((value) => { 
                                            return (
                                                <label key={value} className='filter-checkbox-label'>
                                                    <input type="checkbox" 
                                                        onChange={() => filterAttributeClickHandler(filterName, value)}
                                                        checked={selectedFilters[filterName]?.includes(value) ?? false} className='filter-checkbox' />
                                                    {filterNamesMapping.Values[value] ?? value}
                                                </label>
                                            )
                                        })
                                    }
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
                <div className='filter-footer'>
                    <button onClick={applyFilters} className='default-button w-100'>Применить</button>
                    {filtersCount > 0 && <button onClick={clearFilters} className='w-100 clear-button'>Очистить</button>}
                </div>
            </div>
        </div>
    );
}

export default FiltersComponent;