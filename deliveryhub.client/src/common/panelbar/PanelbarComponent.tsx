import { useState, type FC, type ReactNode } from 'react';
import './PanelbarComponent.scss';

interface PanelbarProps {
    id: number;
    title: string;
    panelIsOpen: boolean;
    children?: ReactNode;
    click: (id: number) => void;
}

const PanelbarComponent: FC<PanelbarProps> = ({ id, title, panelIsOpen, children, click }) => {
    const [isOpen, setIsOpen] = useState(panelIsOpen);

    return (
        <div className="panelbar-wrapper" onClick={() => click(id) }>
            <div className="panelbar-item__title" onClick={() => setIsOpen(!isOpen)}>
                <span className='text_pointer default_name_chapter'>{title}</span>
            </div>
            <div className={`panelbar-item__content ${isOpen ? 'show' : 'hide'}`}>
                <div className="panelbar-content-inner">
                    {children}
                </div>
            </div>
        </div>
    );
}

export default PanelbarComponent;