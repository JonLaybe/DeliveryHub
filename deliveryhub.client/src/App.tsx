import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.scss'
import OrderComponent from './components/orders/OrderComponent'
import HeaderComponent from './shared/layout/header/HeaderComponent'

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
      <HeaderComponent></HeaderComponent>
      <div className='main-container'>
        <OrderComponent></OrderComponent>
      </div>
    </>
  )
}

export default App
