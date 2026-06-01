import { type FC } from 'react';
import './ConterComponent.scss';
import minus_actions from '../../assets/minus_actions.svg';
import pluse_actions from '../../assets/pluse_actions.svg';

interface CounterProps {
    counter: number;
    onClickPlus: () => void;
    onClickMinus: () => void;
}

const ConterComponent: FC<CounterProps> = ({ counter, onClickPlus, onClickMinus }) => {
    return (
        <div className="counter-wrapper">
            <button className='default-button counter_actions' onClick={() => onClickMinus()}>
                <img src={minus_actions} alt="minus" />
            </button>
            <span className='default_text'>{counter}</span>
            <button className='default-button counter_actions' onClick={() => onClickPlus()}>
                <img src={pluse_actions} alt="plus" />
            </button>
        </div>
    );
}

export default ConterComponent;