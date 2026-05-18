import { useState } from "react";
import './FiltersControlComponent.scss';

const SortingFilterComponent = (props: {selectedSort: string, setSelectedSort: (sort: string) => void }) => {
    
    const [showDropdown, setShowDropdown] = useState<boolean>(false);
    const [selectedBtnText, setSelectedBtnText] = useState<string>('По возрастанию цены');

     const handleSortingChange = (sortType: string) => {
        setShowDropdown(false);
        props.setSelectedSort(sortType);
        setSelectedBtnText(sortType === 'PriceAsc' ? 'По возрастанию цены' : 'По убыванию цены');
    }

    return (
        <div className="dropdown-filter">   
                    <button className="filter-button filter-button--sort" onClick={() => setShowDropdown(!showDropdown)}>
                        {selectedBtnText}
                    </button>
                    {showDropdown && (
                        <div className="dropdown-filter-content">
                            <ul className="filter-list">
                                <li className="filter-item">
                                    <div className={`radio-with-text ${props.selectedSort === 'PriceAsc' ? 'selected' : ''}`} onClick={() => handleSortingChange("PriceAsc")}>
                                        <span className="radio"></span>
                                        <span className="text">По возрастанию цены</span>
                                </div>
                            </li>
                            <li className="filter-item">
                                <div className={`radio-with-text ${props.selectedSort === 'PriceDesc' ? 'selected' : ''}`} onClick={() => handleSortingChange("PriceDesc")}>
                                    <span className="radio"></span>
                                    <span className="text">По убыванию цены</span>
                                </div>
                            </li>
                        </ul>
                    </div>)}
                </div>
    );
}

export default SortingFilterComponent;