import { useContext , useState } from "react";
import './FiltersControlComponent.scss';
import { SearchContext } from "../../context/SearchContext";
import type { UUIDTypes } from "uuid";

const CategoriesFilterComponent = (props: { selectedCategoryId: UUIDTypes | undefined, setSelectedCategoryId: (id: UUIDTypes | undefined) => void }) => {
    const { 
        filterCategories,
    } = useContext(SearchContext);
    
    const [showDropdown, setShowDropdown] = useState<boolean>(false);
    const [selectedBtnText, setSelectedBtnText] = useState<string>('Категория');
    const [filterCanClear, setFilterCanClear] = useState<boolean>(false);

    const handleClearFilterState = () => {
        handleCategoryChange(undefined);
        setShowDropdown(false);
    };

    const handleCategoryChange = (categoryId: UUIDTypes | undefined) => {
        props.setSelectedCategoryId(categoryId);
        const category = filterCategories.find(c => c.id === categoryId);
        setSelectedBtnText(category ? category.name : 'Категория');
        setShowDropdown(false);
    
        if (categoryId) {
            setFilterCanClear(true);
        }
        else {
            setFilterCanClear(false);
        }
    };

    return (
        <div className="dropdown-filter">
            <button className="filter-button filter-button--category" onClick={() => {if (!props.selectedCategoryId) setShowDropdown(!showDropdown)} }>
                {selectedBtnText}
                {filterCanClear && (<span className="clear-state" onClick={() => handleClearFilterState()}></span>)}
            </button>
                {showDropdown && (
                    <div className="dropdown-filter-content">
                        <ul className="filter-list">
                            {filterCategories.map(category => (
                                <li key={category.id.toString()} className="filter-item">
                                    <div className={`radio-with-text ${props.selectedCategoryId === category.id ? 'selected' : ''}`} onClick={() => {
                                        handleCategoryChange(category.id);
                                        setShowDropdown(false);
                                    }}>
                                        <span className="radio"></span>
                                        <span className="text">{category.name}</span>
                                    </div>
                                </li>
                            ))}
                        </ul>
                    </div>)}
        </div>
    );
}

export default CategoriesFilterComponent;