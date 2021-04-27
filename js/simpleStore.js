var _store = {};

class SimpleStore {
    constructor(props) {
        if (props) {
            _store = props;
        }
    }
    getContent() {
        return _store;
    }
}
export default SimpleStore