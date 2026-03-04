import './App.scss'
import HeaderComponent from './shared/layout/header/HeaderComponent'
import RoutingComponent from './components/routing/RoutingComponent'
import { BrowserRouter } from 'react-router-dom'

function App() {
  return (
    <BrowserRouter>
      <HeaderComponent />
      <div className='wrapper'>
        <RoutingComponent />
      </div>
    </BrowserRouter>
  )
}

export default App
