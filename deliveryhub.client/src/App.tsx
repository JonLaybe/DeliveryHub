import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.scss'
import HeaderComponent from './shared/layout/header/HeaderComponent'
import OrderComponent from './components/orders/OrderComponent'
import ProductComponent from './components/products/ProductComponent'
import { Provider } from 'react-redux'

function App() {
  return (
    <>
      <HeaderComponent></HeaderComponent>
      <div className='wrapper'>
        <OrderComponent></OrderComponent>
        {/* <ProductComponent></ProductComponent> */}
      </div>
    </>
  )
}

export default App
