import './App.scss'
import HeaderComponent from './shared/layout/header/HeaderComponent'
import RoutingComponent from './components/routing/RoutingComponent'
import { BrowserRouter } from 'react-router-dom'
import { SearchProvider } from './context/SearchContext'
import FiltersComponent from './components/filters/FiltersComponent'

function App() {
  return (
    <BrowserRouter>
      <SearchProvider>
        <HeaderComponent />
        <FiltersComponent />
        <div className='wrapper'>
          <RoutingComponent />
        </div>
      </SearchProvider>
    </BrowserRouter>
  )
}

export default App
